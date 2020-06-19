using System;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Server
{
    public class ObservationsTransfer : BaseScript
    {
        public ObservationsTransfer()
        {
            EventHandlers["Server:RequestObservations"] += new Action<int, Vector3>((request, location) =>
            {
                TriggerClientEvent("Client:RequestObservations", request, location);
            });

            EventHandlers["Server:ReceiveObservations"] += new Action<int, int, string, string, string, string, string, string, int, string, int, int>((local, patient, name, pHeartRate, pBloodPressure, pSpO2, pTemperature, pBloodSugar, pPupils, pRespRate, pEcgRythm, pLungSounds) =>
            {
                TriggerClientEvent("Client:ReceiveObservations", local, patient, name, pHeartRate, pBloodPressure, pSpO2, pTemperature, pBloodSugar, pPupils, pRespRate, pEcgRythm, pLungSounds);
            });
            // 0 = safe, 1 = medium, 2 = high
            EventHandlers["Server:ReceiveUpdate"] += new Action<int, string, string, string, string, string, int>((request, name, obsName, before, after, units, type) =>
            {
                TriggerClientEvent("Client:ReceiveUpdate", request, name, obsName, before, after, units, type);
            });

            EventHandlers["Server:ClearSubscribed"] += new Action<int>((request) =>
            {
                TriggerClientEvent("Client:ClearSubscribed", request);
            });

            EventHandlers["Server:SubmitReminder"] += new Action<int, int, string>((request, local, ob) =>
            {
                TriggerClientEvent("Client:SubmitReminder", request, local, ob);
            });
        }
    }
}
