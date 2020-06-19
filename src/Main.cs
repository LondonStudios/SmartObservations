using System;
using System.Collections.Generic;
using System.Linq;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using Client;
using static CitizenFX.Core.Native.API;

namespace SmartObservations
{
    public class Main : BaseScript
    {
        internal static string heartRate;
        internal static string bloodPressure;
        internal static string bloodPressure2;
        internal static string spO2;
        internal static string temperature;
        internal static int pupils = -1;
        internal static string respRate;
        internal static int ecgRythm = -1;
        internal static int lungSounds = -1;
        internal static string bloodSugar;
        internal static bool setObs = false;
        internal static string patientName;

        internal static string heartRateUnit = "bpm";
        internal static string bloodPressureUnit = "mmHg";
        internal static string spO2Unit = "%";
        internal static string temperatureUnit = "°C";
        internal static string bloodSugarUnit = "mmoL";
        internal static string pupilsUnit = "";
        internal static string respRateUnit = "/min";
        internal static string ecgRythmUnit = "";
        internal static string lungSoundsUnit = "";

        internal static bool metronomeActive = false;
        internal static bool metronomePlaying = false;

        internal static bool awaitingPulseRise = false;

        internal static bool obsUpdates = true;

        internal static int clientId = new Random().Next(25, 18000);
        internal static List<int> subscribedIds = new List<int>();
        public Main()
        {
            RequestTextures();
            EventHandlers["Client:RequestObservations"] += new Action<int, Vector3>((request, location) =>
            {
                if (setObs == true && GetEntityCoords(PlayerPedId(), true).DistanceToSquared(location) < 10.0f && clientId != request)
                {
                    DisplayNotification("Your ~g~Observations ~w~are being taken.");
                    string bloodPressureCombined;
                    if (IsStringNullOrEmpty(bloodPressure) || IsStringNullOrEmpty(bloodPressure2))
                    {
                        bloodPressureCombined = "";
                    }
                    else
                    {
                        bloodPressureCombined = bloodPressure + "/" + bloodPressure2;
                    }
                    TriggerServerEvent("Server:ReceiveObservations", request, clientId, patientName, heartRate, bloodPressureCombined, spO2, temperature, bloodSugar, pupils, respRate, ecgRythm, lungSounds);
                }
            });

            EventHandlers["Client:SubmitReminder"] += new Action<int, int, string>((request, local, ob) =>
            {
                if (request == clientId)
                {
                    PlaySoundFrontend(-1, "Click", "DLC_HEIST_HACKING_SNAKE_SOUNDS", false);
                    Screen.ShowNotification($"~w~Please update your ~b~{ob} ~w~observation.");
                }
            });

            EventHandlers["Client:ReceiveObservations"] += new Action<int, int, string, string, string, string, string, string, int, string, int, int>((local, patient, name, pHeartRate, pBloodPressure, pSpO2, pTemperature, pBloodSugar, pPupils, pRespRate, pEcgRythm, pLungSounds) =>
            {
                if (clientId == local)
                {
                    MenuHandler.GetObsHandler(name, patient, pHeartRate, pBloodPressure, pSpO2, pTemperature, pBloodSugar, pPupils, pRespRate, pEcgRythm, pLungSounds);
                    if (!subscribedIds.Contains(patient))
                    {
                        subscribedIds.Add(patient);
                        DisplayNotification("Observations have been taken. ~w~You will receive further updates. \nMade by ~q~London Studios~w~.");
                    }
                    else
                    {
                        DisplayNotification("Observations have been taken. \nMade by ~q~London Studios~w~.");
                    }
                }
            });

            EventHandlers["Client:ClearSubscribed"] += new Action<int>((request) =>
            {
                if (subscribedIds.Contains(request))
                {
                    subscribedIds.Remove(request);
                    MenuHandler.ClearShows(request);
                }
            });

            EventHandlers["Client:ReceiveUpdate"] += new Action<int, string, string, string, string, string, int>((request, name, obsName, before, after, units, type) =>
            {
                if (subscribedIds.Contains(request))
                {
                    if (obsUpdates == true)
                    {
                        if (type == 1)
                        {
                            SendNuiMessage(string.Format("{{\"submissionType\":\"smartObservations\", \"submissionVolume\":{0}, \"submissionFile\":\"{1}\"}}", (object)0.55f, "mediumpriority"));
                        }
                        else if (type == 2)
                        {
                            SendNuiMessage(string.Format("{{\"submissionType\":\"smartObservations\", \"submissionVolume\":{0}, \"submissionFile\":\"{1}\"}}", (object)0.55f, "highpriority"));
                        }
                        DisplayNotification($"~g~Obs: ~w~ {name}\n~b~{obsName}\n~g~Before: ~w~{before} {units}\n~g~After: ~w~{after} {units}");
                    }
                }
            });

            RegisterCommand("obs", new Action<int, List<object>, string>((source, args, raw) =>
            {
                TriggerServerEvent("Server:RequestObservations", clientId, GetEntityCoords(PlayerPedId(), true));
            }), false);

            TriggerEvent("chat:addSuggestion", "/obs", "Begin observations on another person.");
            TriggerEvent("chat:addSuggestion", "/setobs", "Set your observations.");
            TriggerEvent("chat:addSuggestion", "/obsupdates", "Toggle observation updates");
        }


        [Command("setobs")]
        public void ObsMenu()
        {
            MenuHandler.Toggle();
        }

        [Command("obsupdates")]
        public void ToggleObsUpdates()
        {
            if (obsUpdates == true)
            {
                obsUpdates = false;
                DisplayNotification("Observation Updates Disabled");
            }
            else
            {
                obsUpdates = true;
                DisplayNotification("Observation Updates Enabled");
            }
        }

        private async void RequestTextures()
        {
            RequestStreamedTextureDict("CHAR_AMANDA", true);
            while (!HasStreamedTextureDictLoaded("CHAR_AMANDA"))
            {
                await Delay(0);
            }
        }

        private static void DisplayNotification(string text)
        {
            SetNotificationTextEntry("STRING");
            AddTextComponentString(text);
            SetNotificationMessage("CHAR_AMANDA", "CHAR_AMANDA", false, 1, "LIFEPAK", "Observations");
            EndTextCommandThefeedPostTicker(false, false);
        }

        internal static void ResetObservations()
        {
            TriggerServerEvent("Server:ClearSubscribed", clientId);
            heartRate = "";
            bloodPressure = "";
            bloodPressure2 = "";
            spO2 = "";
            temperature = "";
            pupils = -1;
            respRate = "";
            ecgRythm = -1;
            lungSounds = -1;
            bloodSugar = "";
            patientName = "";
            setObs = false;
            Main.DisplayNotification("Your observations have been reset");
            MenuHandler.ReloadMenu("set");
        }

        internal static void RequestCivUpdate(int request, string obs)
        {
            TriggerServerEvent("Server:SubmitReminder", request, clientId, obs);
        }

        internal static void ListUpdates(string name, string before, string after, string units)
        {
            TriggerServerEvent("Server:ReceiveUpdate", clientId, patientName, name, before, after, units);
        }

        internal static async void InputHandler(string prompt, string obs, string defaultText)
        {
            AddTextEntry("FMMC_KEY_TIP1", prompt);
            DisplayOnscreenKeyboard(1, "FMMC_KEY_TIP1", "", defaultText, "", "", "", 10);
            while (UpdateOnscreenKeyboard() != 1 && UpdateOnscreenKeyboard() != 2)
            {
                await Delay(500);
            }
            var playerName = patientName;
            var intConvert = 0;
            var type = 0;
            if (UpdateOnscreenKeyboard() != 2)
            {
                setObs = true;
                if (obs.ToLower() == "heartrate")
                {
                    var before = Main.heartRate;
                    Main.heartRate = GetOnscreenKeyboardResult();
                    if(!Int32.TryParse(Main.heartRate, out intConvert))
                    {
                        Screen.ShowNotification("Invalid Observation result");
                        Main.heartRate = before;
                        PlaySoundFrontend(-1, "Place_Prop_Fail", "DLC_Dmod_Prop_Editor_Sounds", false);
                    }
                    else
                    {
                        if (intConvert < 1 && awaitingPulseRise == false)
                        {
                            Screen.ShowNotification("~b~Warning: ~w~You are entering a ~r~cardiac arrest.");
                            type = 2;
                            awaitingPulseRise = true;
                        }
                        else if (intConvert < 40 || intConvert > 120)
                        {
                            type = 2;
                            Screen.ShowNotification("~b~Note: ~w~You have entered a ~r~high risk ~w~observation.");
                        }
                        else if (intConvert < 60 || intConvert > 100)
                        {
                            if (awaitingPulseRise == true)
                            {
                                type = 2;
                                awaitingPulseRise = false;
                            }
                            else
                            {
                                type = 1;
                                Screen.ShowNotification("~b~Note: ~w~You have entered a ~y~medium risk ~w~observation.");
                            }
                        }
                        TriggerServerEvent("Server:ReceiveUpdate", clientId, playerName, "Heart Rate", before, Main.heartRate, Main.heartRateUnit, type);
                    }
                    MenuHandler.ReloadMenu("set");
                }
                else if (obs.ToLower() == "bloodpressure")
                {
                    var before = Main.bloodPressure;
                    Main.bloodPressure = GetOnscreenKeyboardResult();
                    if (!Int32.TryParse(Main.bloodPressure, out intConvert))
                    {
                        Screen.ShowNotification("Invalid Observation result");
                        Main.bloodPressure = before;
                        PlaySoundFrontend(-1, "Place_Prop_Fail", "DLC_Dmod_Prop_Editor_Sounds", false);
                    }
                    else
                    {
                        if (IsStringNullOrEmpty(Main.bloodPressure2))
                        {
                            if (intConvert < 90 || intConvert > 150)
                            {
                                type = 2;
                                Screen.ShowNotification("~b~Note: ~w~You have entered a ~r~high risk ~w~observation.");
                            }
                            else if (intConvert < 119 || intConvert > 1190)
                            {
                                type = 1;
                                Screen.ShowNotification("~b~Note: ~w~You have entered a ~y~medium risk ~w~observation.");
                            }
                            TriggerServerEvent("Server:ReceiveUpdate", clientId, playerName, "Blood Pressure (Systolic)", before + "/" + Main.bloodPressure2, Main.bloodPressure + "/" + Main.bloodPressure2, Main.bloodPressureUnit, type);
                        }
                        else
                        {
                            if (intConvert < Convert.ToInt32(Main.bloodPressure2))
                            {
                                Screen.ShowNotification("Invalid Observation result");
                                PlaySoundFrontend(-1, "Place_Prop_Fail", "DLC_Dmod_Prop_Editor_Sounds", false);
                            }
                            else
                            {
                                if (intConvert < 90 || intConvert > 150)
                                {
                                    type = 2;
                                    Screen.ShowNotification("~b~Note: ~w~You have entered a ~r~high risk ~w~observation.");
                                }
                                else if (intConvert < 119 || intConvert > 1190)
                                {
                                    type = 1;
                                    Screen.ShowNotification("~b~Note: ~w~You have entered a ~y~medium risk ~w~observation.");
                                }
                                TriggerServerEvent("Server:ReceiveUpdate", clientId, playerName, "Blood Pressure (Systolic)", before + "/" + Main.bloodPressure2, Main.bloodPressure + "/" + Main.bloodPressure2, Main.bloodPressureUnit, type);
                            }
                        }
                    }
                    Main.InputHandler("Enter Blood Pressure (Second):", "bloodpressure2", Main.bloodPressure2);
                    MenuHandler.ReloadMenu("set");
                }
                else if (obs.ToLower() == "bloodpressure2")
                {
                    var before = Main.bloodPressure2;
                    Main.bloodPressure2 = GetOnscreenKeyboardResult();
                    if (!Int32.TryParse(Main.bloodPressure2, out intConvert))
                    {
                        Screen.ShowNotification("Invalid Observation result");
                        Main.bloodPressure2 = before;
                        PlaySoundFrontend(-1, "Place_Prop_Fail", "DLC_Dmod_Prop_Editor_Sounds", false);
                    }
                    else
                    {
                        if (intConvert > Convert.ToInt32(Main.bloodPressure))
                        {
                            Screen.ShowNotification("Invalid Observation result");
                            Main.bloodPressure2 = before;
                            PlaySoundFrontend(-1, "Place_Prop_Fail", "DLC_Dmod_Prop_Editor_Sounds", false);
                        }
                        else
                        {
                            if (intConvert < 60 || intConvert > 100)
                            {
                                type = 2;
                                Screen.ShowNotification("~b~Note: ~w~You have entered a ~r~high risk ~w~observation.");
                            }
                            else if (intConvert < 80 || intConvert > 80)
                            {
                                type = 1;
                                Screen.ShowNotification("~b~Note: ~w~You have entered a ~y~medium risk ~w~observation.");
                            }
                            TriggerServerEvent("Server:ReceiveUpdate", clientId, playerName, "Blood Pressure (Diastolic)", Main.bloodPressure + "/" + before, Main.bloodPressure + "/" + Main.bloodPressure2, Main.bloodPressureUnit, type);
                        }
                    }
                    MenuHandler.ReloadMenu("set");
                }

                else if (obs.ToLower() == "spo2")
                {
                    var before = Main.spO2;
                    Main.spO2 = GetOnscreenKeyboardResult();
                    if (!Int32.TryParse(Main.spO2, out intConvert))
                    {
                        Screen.ShowNotification("Invalid Observation result");
                        Main.spO2 = before;
                        PlaySoundFrontend(-1, "Place_Prop_Fail", "DLC_Dmod_Prop_Editor_Sounds", false);
                    }
                    else
                    {
                        if (intConvert < 90)
                        {
                            type = 2;
                            Screen.ShowNotification("~b~Note: ~w~You have entered a ~r~high risk ~w~observation.");
                        }
                        else if (intConvert < 95)
                        {
                            type = 1;
                            Screen.ShowNotification("~b~Note: ~w~You have entered a ~y~medium risk ~w~observation.");
                        }
                        else if (intConvert < 101)
                        {
                            type = 0;
                        }
                        else
                        {
                            Screen.ShowNotification("Invalid Observation result");
                            Main.spO2 = before;
                        }
                        TriggerServerEvent("Server:ReceiveUpdate", clientId, playerName, "SpO2", before, Main.spO2, Main.spO2Unit, type);
                    }
                    MenuHandler.ReloadMenu("set");
                }
                else if (obs.ToLower() == "temperature")
                {
                    float floatConvert;
                    var before = Main.temperature;
                    Main.temperature = GetOnscreenKeyboardResult();
                    if (!float.TryParse(Main.temperature, out floatConvert))
                    {
                        Screen.ShowNotification("Invalid Observation result");
                        Main.temperature = before;
                        PlaySoundFrontend(-1, "Place_Prop_Fail", "DLC_Dmod_Prop_Editor_Sounds", false);
                    }
                    else
                    {
                        if (floatConvert < 35.5 || floatConvert > 39)
                        {
                            type = 2;
                            Screen.ShowNotification("~b~Note: ~w~You have entered a ~r~high risk ~w~observation.");
                        }
                        else if (floatConvert < 36.5 || floatConvert > 37.5)
                        {
                            type = 1;
                            Screen.ShowNotification("~b~Note: ~w~You have entered a ~y~medium risk ~w~observation.");
                        }
                        else
                        {
                            type = 0;
                        }
                        TriggerServerEvent("Server:ReceiveUpdate", clientId, playerName, "Temperature", before, Main.temperature, Main.temperatureUnit, type);

                    }
                    MenuHandler.ReloadMenu("set");
                }
                else if (obs.ToLower() == "resprate")
                {
                    var before = Main.respRate;
                    Main.respRate = GetOnscreenKeyboardResult();
                    if (!Int32.TryParse(Main.respRate, out intConvert))
                    {
                        Screen.ShowNotification("Invalid Observation result");
                        Main.respRate = before;
                        PlaySoundFrontend(-1, "Place_Prop_Fail", "DLC_Dmod_Prop_Editor_Sounds", false);
                    }
                    else
                    {
                        if (intConvert < 8 || intConvert > 28)
                        {
                            type = 2;
                            Screen.ShowNotification("~b~Note: ~w~You have entered a ~r~high risk ~w~observation.");
                        }
                        else if (intConvert <12 || intConvert > 20)
                        {
                            type = 1;
                            Screen.ShowNotification("~b~Note: ~w~You have entered a ~y~medium risk ~w~observation.");
                        }
                        else
                        {
                            type = 0;
                        }
                        TriggerServerEvent("Server:ReceiveUpdate", clientId, playerName, "Resp Rate", before, Main.respRate, Main.respRateUnit, type);
                    }
                    MenuHandler.ReloadMenu("set");
                }
                else if (obs.ToLower() == "bloodsugar")
                {
                    var before = Main.bloodSugar;
                    Main.bloodSugar = GetOnscreenKeyboardResult();
                    if (!Int32.TryParse(Main.bloodSugar, out intConvert))
                    {
                        Screen.ShowNotification("Invalid Observation result");
                        Main.bloodSugar = before;
                        PlaySoundFrontend(-1, "Place_Prop_Fail", "DLC_Dmod_Prop_Editor_Sounds", false);
                    }
                    else
                    {
                        if (intConvert < 2 || intConvert > 12)
                        {
                            type = 2;
                            Screen.ShowNotification("~b~Note: ~w~You have entered a ~r~high risk ~w~observation.");
                        }
                        else if (intConvert < 4 || intConvert > 8)
                        {
                            type = 1;
                            Screen.ShowNotification("~b~Note: ~w~You have entered a ~y~medium risk ~w~observation.");
                        }
                        else
                        {
                            type = 0;
                        }
                        TriggerServerEvent("Server:ReceiveUpdate", clientId, playerName, "Blood Sugar", before, Main.bloodSugar, Main.bloodSugarUnit, type);
                    }
                    MenuHandler.ReloadMenu("set");
                }
                else if (obs.ToLower() == "patientname")
                {
                    Main.patientName = GetOnscreenKeyboardResult();
                    MenuHandler.ReloadMenu("set");
                }
                await Delay(500);
            }
        }
        
    }
}
