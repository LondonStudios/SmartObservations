using System;
using NativeUI;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using SmartObservations;
using System.Collections.Generic;
using GTA;
using System.Web.UI.WebControls;

namespace Client
{
    public static class MenuHandler
    {
        private static UIMenu menu;
        private static MenuPool pool;

        private static List<object> pupilTypes = new List<object>() { "PERL", "Dilated", "Fixed", "Constricted", "Absent", "False", "F&D", "Closed" };
        private static List<object> lungSoundsTypes = new List<object>() { "Clear", "Wheezing", "Bubbling", "Absent" };
        private static List<object> ecgRythmTypes = new List<object>() { "NSR", "Tachycardia", "Bradycardia", "Asystole", "Pulseless Electrical Activity (PEA)", "Ventricular Tachycardia (VT)", "Ventricular Fibrilation (VF)", "ST-segment elevation", "Atrial Fibrilation (AF)" };

        private static int viewingClientId;

        private static string name;
        private static string pHeartRate;
        private static string pBloodPressure;
        private static string pSpO2;
        private static string pTemperature;
        private static string pBloodSugar;
        private static int pPupils;
        private static string pRespRate;
        private static int pEcgRythm;
        private static int pLungSounds;

        private static List<int> showHeartRate = new List<int>() {};
        private static List<int> showBloodPressure = new List<int>() { };
        private static List<int> showSpO2 = new List<int>() { };
        private static List<int> showTemperature = new List<int>() { };
        private static List<int> showBloodSugar = new List<int>() { };
        private static List<int> showPupils = new List<int>() { };
        private static List<int> showRespRate = new List<int>() { };
        private static List<int> showEcgRythm = new List<int>() { };
        private static List<int> showLungSounds = new List<int>() { };

        static MenuHandler()
        {
            pool = new MenuPool()
            {
                MouseEdgeEnabled = false,
                ControlDisablingEnabled = false
            };
            menu = new UIMenu("Observations", "Menu")
            {
                MouseControlsEnabled = false,
                MouseEdgeEnabled = false,
                ControlDisablingEnabled = false
            };
            pool.Add(menu);
            pool.RefreshIndex();
        }

        internal static async void Toggle()
        {
            if (pool.IsAnyMenuOpen())
            {
                pool.CloseAllMenus();
            }
            else
            {
                menu.Visible = true;
                pool.RefreshIndex();
                menu.Clear();
                SetObservations(menu);
                while (pool.IsAnyMenuOpen())
                {
                    pool.ProcessMenus();
                    await BaseScript.Delay(0);
                }
            }
        }

        internal static async void ReloadMenu(string type)
        {
            pool.RefreshIndex();
            menu.Clear();
            if (type == "set")
            {
                SetObservations(menu);
            }
            else if (type == "get")
            {
                GetObservations(menu);
            }
            await BaseScript.Delay(0);
        }

        internal static void ClearShows(int id)
        {
            if (showBloodPressure.Contains(id))
            {
                showBloodPressure.Remove(id);
            }
            if (showHeartRate.Contains(id))
            {
                showHeartRate.Remove(id);
            }
            if (showBloodSugar.Contains(id))
            {
                showBloodSugar.Remove(id);
            }
            if (showSpO2.Contains(id))
            {
                showSpO2.Remove(id);
            }
            if (showTemperature.Contains(id))
            {
                showTemperature.Remove(id);
            }
            if (showPupils.Contains(id))
            {
                showPupils.Remove(id);
            }
            if (showLungSounds.Contains(id))
            {
                showLungSounds.Remove(id);
            }
            if (showRespRate.Contains(id))
            {
                showRespRate.Remove(id);
            }
            if (showEcgRythm.Contains(id))
            {
                showEcgRythm.Remove(id);
            }
        }

        internal static async void GetObsHandler(string eName, int patient, string eHeartRate, string eBloodPressure, string eSpO2, string eTemperature, string eBloodSugar, int ePupils, string eRespRate, int eEcgRythm, int eLungSounds)
        {
            menu.Visible = true;
            pool.RefreshIndex();
            menu.Clear();

            //Menu Handler Fields - Local storage
            name = eName;
            pHeartRate = eHeartRate;
            pBloodPressure = eBloodPressure;
            pSpO2 = eSpO2;
            pTemperature = eTemperature;
            pBloodSugar = eBloodSugar;
            pPupils = ePupils;
            pRespRate = eRespRate;
            pEcgRythm = eEcgRythm;
            pLungSounds = eLungSounds;

            viewingClientId = patient;

            GetObservations(menu);
            while (pool.IsAnyMenuOpen())
            {
                pool.ProcessMenus();
                await BaseScript.Delay(0);
            }
        }


        private static void GetObservations(UIMenu menu)
        {
            menu.Title.Caption = "Observations";
            menu.Subtitle.Caption = $"Patient Name: {name}";
            menu.Subtitle.Centered = true;
            var getHeartRate = new UIMenuItem("Heart Rate:");
            var getBloodPressure = new UIMenuItem("Blood Pressure:");
            var getSpO2 = new UIMenuItem("SpO2:");
            var getTemperature= new UIMenuItem("Temperature:");
            var getBloodSugar = new UIMenuItem("Blood Sugar:");
            var getPupils = new UIMenuItem("Pupils:");
            var getRespRate = new UIMenuItem("Resp Rate:");
            var getEcgRythm = new UIMenuItem("ECG Rhythm:");
            var getLungSounds = new UIMenuItem("Lung Sounds:");
            var disableUpdates = new UIMenuItem("Disable Updates", "You will stop receiving updates from this patient");
            menu.AddItem(getHeartRate);
            menu.AddItem(getBloodPressure);
            menu.AddItem(getSpO2);
            menu.AddItem(getTemperature);
            menu.AddItem(getBloodSugar);
            menu.AddItem(getPupils);
            menu.AddItem(getRespRate);
            menu.AddItem(getEcgRythm);
            menu.AddItem(getLungSounds);
            menu.AddItem(disableUpdates);

            menu.OnItemSelect += (sender, item, index) =>
            {
                if (item == getHeartRate)
                {
                    if (showHeartRate.Contains(viewingClientId))
                    {
                        Main.RequestCivUpdate(viewingClientId, "Heart Rate");
                    }
                    else
                    {
                        showHeartRate.Add(viewingClientId);
                        ReloadMenu("get");
                    }
                }
                else if (item == getBloodPressure)
                {
                    if (showBloodPressure.Contains(viewingClientId))
                    {
                        Main.RequestCivUpdate(viewingClientId, "Blood Pressure");
                    }
                    else
                    {
                        showBloodPressure.Add(viewingClientId);
                        ReloadMenu("get");
                    }
                }
                else if (item == getSpO2)
                {
                    if (showSpO2.Contains(viewingClientId))
                    {
                        Main.RequestCivUpdate(viewingClientId, "SpO2");
                    }
                    else
                    {
                        showSpO2.Add(viewingClientId);
                        ReloadMenu("get");
                    }
                }
                else if (item == getTemperature)
                {
                    if (showTemperature.Contains(viewingClientId))
                    {
                        Main.RequestCivUpdate(viewingClientId, "Temperature");
                    }
                    else
                    {
                        showTemperature.Add(viewingClientId);
                        ReloadMenu("get");
                    }
                }
                else if (item == getRespRate)
                {
                    if (showRespRate.Contains(viewingClientId))
                    {
                        Main.RequestCivUpdate(viewingClientId, "Resp Rate");
                    }
                    else
                    {
                        showRespRate.Add(viewingClientId);
                        ReloadMenu("get");
                    }
                }
                else if (item == getBloodSugar)
                {
                    if (showBloodSugar.Contains(viewingClientId))
                    {
                        Main.RequestCivUpdate(viewingClientId, "Blood Sugar");
                    }
                    else
                    {
                        showBloodSugar.Add(viewingClientId);
                        ReloadMenu("get");
                    }
                }
                else if (item == getEcgRythm)
                {
                    if (showEcgRythm.Contains(viewingClientId))
                    {
                        Main.RequestCivUpdate(viewingClientId, "ECG Rhythm");
                    }
                    else
                    {
                        showEcgRythm.Add(viewingClientId);
                        ReloadMenu("get");
                    }
                }
                else if (item == getLungSounds)
                {
                    if (showLungSounds.Contains(viewingClientId))
                    {
                        Main.RequestCivUpdate(viewingClientId, "Lung Sounds");
                    }
                    else
                    {
                        showLungSounds.Add(viewingClientId);
                        ReloadMenu("get");
                    }
                }
                else if (item == getPupils)
                {
                    if (showPupils.Contains(viewingClientId))
                    {
                        Main.RequestCivUpdate(viewingClientId, "Pupils");
                    }
                    else
                    {
                        showPupils.Add(viewingClientId);
                        ReloadMenu("get");
                    }
                }
                else if (item == disableUpdates)
                {
                    Main.subscribedIds.Remove(viewingClientId);
                    ClearShows(viewingClientId);
                    Toggle();
                }
            };

            if (showHeartRate.Contains(viewingClientId))
            {
                if (IsStringNullOrEmpty(pHeartRate))
                {
                    getHeartRate.SetRightLabel("Observing");
                }
                else
                {
                    getHeartRate.SetRightBadge(UIMenuItem.BadgeStyle.Tick);
                    getHeartRate.SetRightLabel(pHeartRate + Main.heartRateUnit);
                }
            }
            else
            {
                getHeartRate.SetRightLabel("Observe");
            }
            if (showBloodPressure.Contains(viewingClientId))
            {
                if (IsStringNullOrEmpty(pBloodPressure))
                {
                    getBloodPressure.SetRightLabel("Observing");
                }
                else
                {
                    getBloodPressure.SetRightBadge(UIMenuItem.BadgeStyle.Tick);
                    getBloodPressure.SetRightLabel(pBloodPressure + Main.bloodPressureUnit);
                } 
            }
            else
            {
                getBloodPressure.SetRightLabel("Observe");
            }
            if (showSpO2.Contains(viewingClientId))
            {
                if (IsStringNullOrEmpty(pSpO2))
                {
                    getSpO2.SetRightLabel("Observing");
                }
                else
                {
                    getSpO2.SetRightBadge(UIMenuItem.BadgeStyle.Tick);
                    getSpO2.SetRightLabel(pSpO2 + Main.spO2Unit);
                }
            }
            else
            {
                getSpO2.SetRightLabel("Observe");
            }
            if (showTemperature.Contains(viewingClientId))
            {
                if (IsStringNullOrEmpty(pTemperature))
                {
                    getTemperature.SetRightLabel("Observing");
                }
                else
                {
                    getTemperature.SetRightBadge(UIMenuItem.BadgeStyle.Tick);
                    getTemperature.SetRightLabel(pTemperature + Main.temperatureUnit);
                }
            }
            else
            {
                getTemperature.SetRightLabel("Observe");
            }
            if (showBloodSugar.Contains(viewingClientId))
            {
                if (IsStringNullOrEmpty(pBloodSugar))
                {
                    getBloodSugar.SetRightLabel("Observing");
                }
                else
                {
                    getBloodSugar.SetRightBadge(UIMenuItem.BadgeStyle.Tick);
                    getBloodSugar.SetRightLabel(pBloodSugar + Main.bloodSugarUnit);
                }
            }
            else
            {
                getBloodSugar.SetRightLabel("Observe");
            }
            if (showPupils.Contains(viewingClientId))
            {
                if ((pPupils == -1))
                {
                    getPupils.SetRightLabel("Observing");
                }
                else
                {
                    getPupils.SetRightBadge(UIMenuItem.BadgeStyle.Tick);
                    getPupils.SetRightLabel(pupilTypes[pPupils] + Main.pupilsUnit);
                }
            }
            else
            {
                getPupils.SetRightLabel("Observe");
            }
            if (showRespRate.Contains(viewingClientId))
            {
                if (IsStringNullOrEmpty(pRespRate))
                {
                    getRespRate.SetRightLabel("Observing");
                }
                else
                {
                    getRespRate.SetRightBadge(UIMenuItem.BadgeStyle.Tick);
                    getRespRate.SetRightLabel(pRespRate + Main.respRateUnit);
                }
                
            }
            else
            {
                getRespRate.SetRightLabel("Observe");
            }
            if (showEcgRythm.Contains(viewingClientId))
            {
                if ((pEcgRythm == -1))
                {
                    getEcgRythm.SetRightLabel("Observing");
                }
                else
                {
                    getEcgRythm.SetRightBadge(UIMenuItem.BadgeStyle.Tick);
                    getEcgRythm.SetRightLabel(ecgRythmTypes[pEcgRythm] + Main.ecgRythmUnit);
                }

            }
            else
            {
                getEcgRythm.SetRightLabel("Observe");
            }
            if (showLungSounds.Contains(viewingClientId))
            {
                if ((pLungSounds == -1))
                {
                    getLungSounds.SetRightLabel("Observing");
                }
                else
                {
                    getLungSounds.SetRightBadge(UIMenuItem.BadgeStyle.Tick);
                    getLungSounds.SetRightLabel(lungSoundsTypes[pLungSounds] + Main.lungSoundsUnit);
                } 
            }
            else
            {
                getLungSounds.SetRightLabel("Observe");
            }
        }

        private static void SetObservations(UIMenu menu)
        {
            menu.Title.Caption = "Observations";
            menu.Subtitle.Caption = $"Set your observations!";
            menu.Subtitle.Centered = true;
            var setHeartRate = new UIMenuItem("Set Heart Rate:");
            var setBloodPressure = new UIMenuItem("Set Blood Pressure:");
            var setSpO2 = new UIMenuItem("Set SpO2:");
            var setTemperature = new UIMenuItem("Set Temperature:");
            var setBloodSugar = new UIMenuItem("Set Blood Sugar:");
            UIMenuListItem setPupils = new UIMenuListItem("Set Pupils", pupilTypes, 0);
            var setRespRate = new UIMenuItem("Set Resp Rate:");
            UIMenuListItem setEcgRythm = new UIMenuListItem("Set ECG Rhythm", ecgRythmTypes, 0);
            UIMenuListItem setLungSounds = new UIMenuListItem("Set Lung Sounds", lungSoundsTypes, 0);
            var setPatientName = new UIMenuItem("Set Patient Name:");
            var resetObservations = new UIMenuItem("Reset Observations");

            menu.AddItem(setHeartRate);
            menu.AddItem(setBloodPressure);
            menu.AddItem(setSpO2);
            menu.AddItem(setTemperature);
            menu.AddItem(setBloodSugar);
            menu.AddItem(setPupils);
            menu.AddItem(setRespRate);
            menu.AddItem(setEcgRythm);
            menu.AddItem(setLungSounds);
            menu.AddItem(setPatientName);
            menu.AddItem(resetObservations);
            if (Main.ecgRythm != -1)
            {
                setEcgRythm.Index = Main.ecgRythm;
            }
            if (Main.lungSounds != -1)
            {
                setLungSounds.Index = Main.lungSounds;
            }
            if (Main.pupils != -1)
            {
                setPupils.Index = Main.pupils;
            }

            setEcgRythm.OnListSelected += (_arg, _selection) =>
            {
                var before = Main.ecgRythm;
                string beforeString;
                if (before == -1)
                {
                    beforeString = "";
                }
                else
                {
                    beforeString = Convert.ToString(ecgRythmTypes[before]);
                }
                Main.ListUpdates("ECG Rythm", beforeString, Convert.ToString(ecgRythmTypes[_selection]), Main.ecgRythmUnit);
                Main.ecgRythm = _selection;
            };

            setPupils.OnListSelected += (_arg, _selection) =>
            {
                var before = Main.pupils;
                string beforeString;
                if (before == -1)
                {
                    beforeString = "";
                }
                else
                {
                    beforeString = Convert.ToString(pupilTypes[before]);
                }
                Main.ListUpdates("Pupils", beforeString, Convert.ToString(pupilTypes[_selection]), Main.pupilsUnit);
                Main.pupils = _selection;
            };

            setLungSounds.OnListSelected += (_arg, _selection) =>
            {
                var before = Main.lungSounds;
                string beforeString;
                if (before == -1)
                {
                    beforeString = "";
                }
                else
                {
                    beforeString = Convert.ToString(lungSoundsTypes[before]);
                }
                Main.ListUpdates("Lung Sounds", beforeString, Convert.ToString(lungSoundsTypes[_selection]), Main.lungSoundsUnit);
                Main.lungSounds = _selection;
            };

            if (!IsStringNullOrEmpty(Main.heartRate))
            {
                setHeartRate.SetRightBadge(UIMenuItem.BadgeStyle.Tick);
                setHeartRate.SetRightLabel(Main.heartRate + Main.heartRateUnit);
            }
            if (!IsStringNullOrEmpty(Main.bloodPressure))
            {
                setBloodPressure.SetRightBadge(UIMenuItem.BadgeStyle.Tick);
                setBloodPressure.SetRightLabel(Main.bloodPressure + "/" + Main.bloodPressure2 + Main.bloodPressureUnit);
            }
            if (!IsStringNullOrEmpty(Main.spO2))
            {
                setSpO2.SetRightBadge(UIMenuItem.BadgeStyle.Tick);
                setSpO2.SetRightLabel(Main.spO2 + Main.spO2Unit);
            }
            if (!IsStringNullOrEmpty(Main.temperature))
            {
                setTemperature.SetRightBadge(UIMenuItem.BadgeStyle.Tick);
                setTemperature.SetRightLabel(Main.temperature + Main.temperatureUnit);
            }
            if (!IsStringNullOrEmpty(Main.bloodSugar))
            {
                setBloodSugar.SetRightBadge(UIMenuItem.BadgeStyle.Tick);
                setBloodSugar.SetRightLabel(Main.bloodSugar + Main.bloodSugarUnit);
            }
            if (!IsStringNullOrEmpty(Main.respRate))
            {
                setRespRate.SetRightBadge(UIMenuItem.BadgeStyle.Tick);
                setRespRate.SetRightLabel(Main.respRate + Main.respRateUnit);
            }
            if (!IsStringNullOrEmpty(Main.patientName))
            {
                setPatientName.SetRightBadge(UIMenuItem.BadgeStyle.Tick);
                setPatientName.SetRightLabel(Main.patientName);
            }

            menu.OnItemSelect += (sender, item, index) =>
            {
                if (item == setHeartRate)
                {
                    Main.InputHandler("Enter Heart Rate:", "heartRate", Main.heartRate);
                }
                else if (item == setBloodPressure)
                {
                    Main.InputHandler("Enter Blood Pressure (First):", "bloodpressure", Main.bloodPressure);
                }
                else if (item == setSpO2)
                {
                    Main.InputHandler("Enter SpO2:", "spO2", Main.spO2);
                }
                else if (item == setTemperature)
                {
                    Main.InputHandler("Enter Temperature:", "temperature", Main.temperature);
                }
                else if (item == setRespRate)
                {
                    Main.InputHandler("Enter Resp Rate:", "respRate", Main.respRate);
                }
                else if (item == setBloodSugar)
                {
                    Main.InputHandler("Enter Blood Sugar:", "bloodsugar", Main.bloodSugar);
                }
                else if (item == setPatientName)
                {
                    Main.InputHandler("Enter Patient Name:", "patientname", Main.patientName);
                }
                else if (item == resetObservations)
                {
                    Main.ResetObservations();
                }
            };
        }
    }
}
