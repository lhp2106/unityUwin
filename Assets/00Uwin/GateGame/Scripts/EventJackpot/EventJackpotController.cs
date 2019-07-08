using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventJackpotController : MonoBehaviour
{
    private Database dataBase;
    private MEventGetBigJackpotInfo eventInfoJackpot;

    private void Start()
    {
        dataBase = Database.Instance;
        WebServiceController.Instance.OnWebServiceResponse += OnWebServiceResponse;
        SendGetEventJackpot();
        StartCoroutine(GetEventJackpotLoop());
    }

    #region WebServiceController

    private void OnWebServiceResponse(WebServiceCode.Code code, WebServiceStatus.Status status, string data)
    {
        switch (code)
        {
            case WebServiceCode.Code.GetBigJackpotInfoFarm:
                try
                {
                    eventInfoJackpot = LitJson.JsonMapper.ToObject<MEventGetBigJackpotInfo>(data);
                    AddDatabase((int)GameId.SLOT_NONGTRAI, eventInfoJackpot);
                }
                catch
                {
                    eventInfoJackpot = new MEventGetBigJackpotInfo
                    {
                        list = new List<InfoEventJackpot>(),
                        IsEvent = false,
                    };
                }
                break;
            case WebServiceCode.Code.GetBigJackpotInfoMafia:
                try
                {
                    eventInfoJackpot = LitJson.JsonMapper.ToObject<MEventGetBigJackpotInfo>(data);
                    AddDatabase((int)GameId.SLOT_MAFIA, eventInfoJackpot);
                }
                catch
                {
                    eventInfoJackpot = new MEventGetBigJackpotInfo
                    {
                        list = new List<InfoEventJackpot>(),
                        IsEvent = false,
                    };
                }
                break;
            case WebServiceCode.Code.GetBigJackpotInfo25Line:
                try
                {
                    eventInfoJackpot = LitJson.JsonMapper.ToObject<MEventGetBigJackpotInfo>(data);
                    AddDatabase((int)GameId.SLOT_HAIVUONG, eventInfoJackpot);
                }
                catch
                {
                    eventInfoJackpot = new MEventGetBigJackpotInfo
                    {
                        list = new List<InfoEventJackpot>(),
                        IsEvent = false,
                    };
                }
                break;
            //case WebServiceCode.Code.GetBigJackpotInfoHilo:
            //    try
            //    {
            //        eventInfoJackpot = LitJson.JsonMapper.ToObject<MEventGetBigJackpotInfo>(data);
            //        AddDatabase((int)GameId.HIGHLOW, eventInfoJackpot);
            //    }
            //    catch
            //    {
            //        eventInfoJackpot = new MEventGetBigJackpotInfo
            //        {
            //            list = new List<InfoEventJackpot>(),
            //            IsEvent = false,
            //        };
            //    }
            //    break;
            case WebServiceCode.Code.GetBigJackpotInfoMiniPoker:
                try
                {
                    eventInfoJackpot = LitJson.JsonMapper.ToObject<MEventGetBigJackpotInfo>(data);
                    AddDatabase((int)GameId.MINIPOKER, eventInfoJackpot);
                }
                catch
                {
                    eventInfoJackpot = new MEventGetBigJackpotInfo
                    {
                        list = new List<InfoEventJackpot>(),
                        IsEvent = false,
                    };
                }
                break;
            case WebServiceCode.Code.GetBigJackpotInfoVuaBao:
                try
                {
                    eventInfoJackpot = LitJson.JsonMapper.ToObject<MEventGetBigJackpotInfo>(data);
                    AddDatabase((int)GameId.VUABAO, eventInfoJackpot);
                }
                catch
                {
                    eventInfoJackpot = new MEventGetBigJackpotInfo
                    {
                        list = new List<InfoEventJackpot>(),
                        IsEvent = false,
                    };
                }
                break;

        }
    }

    #endregion

    private IEnumerator GetEventJackpotLoop()
    {
        while (true)
        {
            if (dataBase == null)
            {
                dataBase = Database.Instance;
            }

            var timeLoop = dataBase.timeGetInfoEventJackpot;

            if (!dataBase.isGetDataEventJackpot)
            {
                timeLoop = 1f;
            }

            yield return Yielders.Get(timeLoop);
            SendGetEventJackpot();
        }
    }

    private void SendGetEventJackpot()
    {
        SendRequest.SendGetEventBigJackpotFarm();
        SendRequest.SendGetEventBigJackpotMafia();
        SendRequest.SendGetEventBigJackpot25Line();
        //SendRequest.SendGetEventBigJackpotHilo();
        SendRequest.SendGetEventBigJackpotVuaBao();
        SendRequest.SendGetEventBigJackpotMinipoker();
    }

    private void AddDatabase(int key, MEventGetBigJackpotInfo data)
    {
        if (dataBase == null)
        {
            dataBase = Database.Instance;
        }

        dataBase.UpdateEventJackpot(key, data);
    }
}
