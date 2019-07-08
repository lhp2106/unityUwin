using System;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class SRSVuaBaoResultSpin
{
    public double _SpinID;
    public string _SlotsData;
    public string _PrizesData;
    public int _TotalBetValue;
    public double _TotalPrizeValue;
    public bool _IsJackpot;
    public double _Jackpot;
    public double _Balance;
    public int _ResponseStatus;
    public int _IsAutoFreeze;
    public string LuckyData;
    public int TotalJackPot;

    public List<SRSVuaBaoPrizesData> prizesDatas;
    public List<int> itemIds;

    public List<int> GetItems()
    {
        return _SlotsData.Split(',').Select(a => int.Parse(a)).Select(a => a -= 1).ToList();
    }

    public double GetMoneyBonus()
    {
        if (string.IsNullOrEmpty(LuckyData))
            return _TotalPrizeValue;

        var bonus = LuckyData.Split(',');

        return _TotalPrizeValue + int.Parse(bonus.Last());
    }

    public List<int> GetLinesWin()
    {
        List<int> lines = new List<int>();

        if (string.IsNullOrEmpty(_PrizesData))
            return new List<int>();

        var obj = _PrizesData.Split(';');
        foreach (var v in obj)
        {
            var vObj = v.Split(',');
            lines.Add(int.Parse(vObj[0]));
        }

        return lines;
    }

    public void LoadPrizesData()
    {
        prizesDatas = new List<SRSVuaBaoPrizesData>();
        itemIds = _SlotsData.Split(',').Select(a => int.Parse(a)).ToList();

        if (!string.IsNullOrEmpty(_PrizesData))
        {
            var obj = _PrizesData.Split(';');
            foreach (var v in obj)
            {
                var vObj = v.Split(',');

                SRSVuaBaoPrizesData dt = new SRSVuaBaoPrizesData()
                {
                    line = int.Parse(vObj[0]),
                    idBonus = int.Parse(vObj[1]),
                    money = int.Parse(vObj[2]),
                };
                if (dt.idBonus != 10) // truong howpj trungs bonus game thi bo qua ko hien thi
                {
                    prizesDatas.Add(dt);
                }
            }
        }
    }
}


[System.Serializable]
public class SRSVuaBaoPrizesData
{
    public int line;
    public int idBonus;
    public int money;

    public List<int> position;

    public List<int> GetPosition(List<string> mapDatas, List<int> itemIds)
    {
        if (position == null || position.Count <= 0)
        {
            position = new List<int>();

            // load map item position
            List<int> mapPosition = new List<int>();
            List<int> lineItemIds = new List<int>();
            if (mapDatas.Count >= line)
            {
                string map = mapDatas[line - 1];

                //VKDebug.LogError("MAP " + map);

                var obj = map.Split(',');
                foreach (var v in obj)
                {
                    int index = int.Parse(v);
                    mapPosition.Add(index);
                    if (index < itemIds.Count)
                    {
                        lineItemIds.Add(itemIds[index]);
                    }
                }
                //VKDebug.LogError("idBonus " + idBonus);
                //VKDebug.LogError("lineItemIds " + LitJson.JsonMapper.ToJson(lineItemIds));

                // load item position win
                switch (idBonus)
                {
                    case 6: // truong hop 2 item 5
                        for (int i = 0; i < lineItemIds.Count; i++)
                        {
                            if (lineItemIds[i] == 5 || lineItemIds[i] == 1)
                            {
                                position.Add(mapPosition[i]);
                            }
                        }
                        break;
                    case 8: // truong hop 2 item 6
                        for (int i = 0; i < lineItemIds.Count; i++)
                        {
                            if (lineItemIds[i] == 6 || lineItemIds[i] == 1)
                            {
                                position.Add(mapPosition[i]);
                            }
                        }
                        break;
                    case 9: // truong hop 1 item 5 - 1 item 6
                        for (int i = 0; i < lineItemIds.Count; i++)
                        {
                            if (lineItemIds[i] == 5 || lineItemIds[i] == 6 || lineItemIds[i] == 1)
                            {
                                position.Add(mapPosition[i]);
                            }
                        }
                        break;
                    case 10:
                        break;
                    default:
                        position = mapPosition;
                        break;
                }
            }
        }

        //VKDebug.LogError("position " + LitJson.JsonMapper.ToJson(position));
        return position;
        //1 - wild, 2 3 4, 5 6
        /*id giai
        1  Jackpot
        2  3 Rồng
        3  3 Ngọc
        4  3 Xe Đào Mỏ
        5  3 Que Kẹo
          6  2 Que Kẹo
        7  3 Bánh Mì
          8  2 Bánh Mì
          9  1 Bánh mì +1 que kẹo
        10  May mắn (Vương miện+Rồng+Ngọc)*/

    }
}

[System.Serializable]
public class SRSVuaBaoRank
{
    public List<SRSVuaBaoRankItem> data;
}

[System.Serializable]
public class SRSVuaBaoRankItem
{
    public string Username;
    public string CreatedTime;
    public string SpinID;
    public int RoomID;
    public int BetValue;
    public int TotalBetValue;
    public int TotalPrizeValue;

    public string Time
    {
        get
        {
            DateTime dateTime = DateTime.Parse(CreatedTime);
            return dateTime.ToString("HH:mm dd/MM/yyyy");
        }
    }
}

[System.Serializable]
public class SRSVuaBaoHistory
{
    public List<SRSVuaBaoHistoryItem> data;
}

[System.Serializable]
public class SRSVuaBaoHistoryItem
{
    public string Username;
    public string CreatedTime;
    public string SpinID;
    public int RoomID;
    public int BetValue;
    public int TotalLines;
    public int TotalBetValue;
    public int TotalPrizeValue;

    public string Time
    {
        get
        {
            DateTime dateTime = DateTime.Parse(CreatedTime);
            return dateTime.ToString("HH:mm dd/MM/yyyy");
        }
    }
}
