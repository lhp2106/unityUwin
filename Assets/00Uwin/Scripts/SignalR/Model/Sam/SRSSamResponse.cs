using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum SamMoneyType : int
{
    Coin = 0, // coin này ở global là = 2 
    Gold = 1,
}

public enum SamRule : int
{
    DEM_LA = 0,
    NHAT_AN_TAT = 1
}

public enum SamConnectionStatus : int
{
    DISCONNECT = 0,
    CONNECTED = 1,
    REGISTER_LEAVE = 2
}

public enum SamPlayerStatus : int
{
    IN_LOBBY = -1,
    VIEWER = 0,
    PLAYING = 1,
    WAITING_MATCH = 2
}

public enum SamSuite : int
{
    CO = 39,
    RO = 26,
    TEP = 9,
    BICH = 0
}

public enum SamActionName : int
{
    START_GAME = 1000,
    WAIT = -1,
    DANH_BAI = 1,
    BO_LUOT = 2,
    CHAT_BAI = 3,
    THANG_THUA = 4,

    // custom
    //BAO_SAM = 101

}

public enum SamToiTrangType : int
{
    None = -1,
    TuQuyBa = 0,
    BaDoiThongChuaBaBich = 1,
    SanhRong = 2,
    NamDoiThong = 3,
    SauDoiBatKy = 4,
    TuQuyHai = 5
}

public enum SamResultFamily : int
{
    DEN_SAM = -2,
    CONG_VA_THOI = -1,
    CONG = 0,
    THOI_BAI = 1,
    BET = 2,
    NORMAL = 3,
    NHAT = 4,
    TOI_TRANG = 5,
    CHAN_SAM = 6,
    AN_CA_LANG = 7
}

public enum SamCardAttackType : int
{
    ONE_CARD_NORMAL = 0,
    ONE_CARD_TWO = 1,
    PAIR_NORMAL = 2,
    PAIR_TWO = 3,
    THREE_OFAKIND_NORMAL = 4,
    THREE_OFAKIND_TWO = 5,
    FOUR_OFAKIND_NORMAL = 6,
    FOUR_OFAKIND_TWO = 7,
    STRAIGHT = 8,
    THREE_STRAIGHT_PAIR = 9,
    FOUR_STRAIGHT_PAIR = 10
}

public enum SamPlayerResurltStatus : int
{
    AN_SAM = 0,
    BI_CHAT,
    CHAT_SAM,
    CONG,
    DEN_SAM,
    THANG,
    THOI,
    THUA,
    TOI_TRANG,
    TU_QUY
}

[System.Serializable]
public class SRSSamConfig
{
    public int gameId;
    public string urlApi;
    public string urlServerSolo;
    public string urlServerMulti;
    public string hubName;

    public AudioClip audioBackground;

    public AudioClip audioButtonClick;
    public AudioClip audioButtonFail;

    public AudioClip audioAnTien;
    public AudioClip audioBaoSam;
    public AudioClip audioChiaBai;
    public AudioClip audioDanhBai;
    public AudioClip audioDanh2;
    public AudioClip audioHurryup;
    public AudioClip audioMatTien;
    public AudioClip audioThang;
    public AudioClip audioThangDam;
    public AudioClip audioThua;
    public AudioClip audioTicktak;
    public AudioClip audioYourTurn;

    public List<int> goldBets;
    public List<int> coinBets;

    public Sprite[] sprAvatars;
    public Sprite sprAvatarDefault;

    public Sprite[] sprPlayerResultStatus;

    public Sprite GetAvatar(int index)
    {
        if(index < sprAvatars.Length)
        {
            return sprAvatars[index];
        }
        return sprAvatarDefault;
    }

    public Sprite GetPlayerResultStatus(SamPlayerResurltStatus status)
    {
        int index = (int)status;
        if (index < sprPlayerResultStatus.Length)
        {
            return sprPlayerResultStatus[index];
        }
        return null;
    }

    public Sprite GetRandomAvatar()
    {
        if (sprAvatars.Length > 0)
        {
            return sprAvatars[Random.Range(0, sprAvatars.Length)];
        }
        return sprAvatarDefault;
    }

    public string GetErrorMessage(int code)
    {
        string strErr = "";
        switch (code)
        {
            case 3: strErr = "Bạn bị đuổi khỏi phòng do chưa sẵn sàng chơi"; break;
            case -1: strErr = "Không tìm thấy người chơi"; break;
            case -2: strErr = "Người chơi chưa sãn sàng"; break;
            case -3: strErr = "Phòng chơi đã giải tán"; break;
            case -4: strErr = "Không đủ tiền"; break;
            case -5: strErr = "Phòng đã đầy"; break;
            case -6: strErr = "Chưa thể thoát phòng chơi lúc này"; break;
            case -7: strErr = "Nhà cái không đủ tiền cân cửa"; break;
            case -8: strErr = "Số dư không đủ để đặt cửa"; break;
            case -9: strErr = "Không có nhà cái cân cửa"; break;
            case -10: strErr = "Không thể bán cửa"; break;
            case -11: strErr = "Cửa đã được bán"; break;
            case -12: strErr = "Đã ngồi rồi"; break;
            case -13: strErr = "Không trong thời gian đặt cược"; break;
            case -99: strErr = "Lỗi không xác định"; break;
            default: strErr = Helper.GetStringError(code); break;
        }

        return strErr;
    }

    public string GetSamResultFamilyName(SamResultFamily code)
    {
        string strErr = "";
        //switch (code)
        //{
        //    //case SamResultFamily.DEN_SAM: strErr = "Đền sâm"; break;
        //    //case SamResultFamily.CONG_VA_THOI: strErr = "Cống và thối"; break;
        //    //case SamResultFamily.CONG: strErr = "Cóng"; break;
        //    //case SamResultFamily.THOI_BAI: strErr = "Thối"; break;
        //    //case SamResultFamily.BET: strErr = "Bét"; break;
        //    //case SamResultFamily.NHAT: strErr = "Nhất"; break;
        //    //case SamResultFamily.TOI_TRANG: strErr = "Thắng trắng"; break;
        //    //case SamResultFamily.CHAT_SAM: strErr = "Chặt sâm"; break;
        //}

        return strErr;
    }

    public SamPlayerResurltStatus GetSamPlayerResurltStatus(SamResultFamily code)
    {
        switch (code)
        {
            case SamResultFamily.DEN_SAM: return SamPlayerResurltStatus.DEN_SAM;
            case SamResultFamily.CONG_VA_THOI: return SamPlayerResurltStatus.CONG;
            case SamResultFamily.CONG: return SamPlayerResurltStatus.CONG;
            case SamResultFamily.THOI_BAI: return SamPlayerResurltStatus.THOI;
            case SamResultFamily.BET: return SamPlayerResurltStatus.THUA;
            case SamResultFamily.NORMAL: return SamPlayerResurltStatus.THUA;
            case SamResultFamily.NHAT: return SamPlayerResurltStatus.THANG;
            case SamResultFamily.TOI_TRANG: return SamPlayerResurltStatus.TOI_TRANG;
            case SamResultFamily.CHAN_SAM: return SamPlayerResurltStatus.CHAT_SAM;
            case SamResultFamily.AN_CA_LANG: return SamPlayerResurltStatus.AN_SAM;
        }

        return SamPlayerResurltStatus.THUA;
    }

    public SamMoneyType GetMoneyType(int mType)
    {
        if (mType == MoneyType.GOLD)
            return SamMoneyType.Gold;
        else
            return SamMoneyType.Coin;
    }

    public bool IsAttack2(List<int> cardIds)
    {
        return cardIds.Any(a => a >= 48);
    }

    public bool IsAttack2(SamCardAttackType type)
    {
        switch(type)
        {
            case SamCardAttackType.ONE_CARD_TWO:
            case SamCardAttackType.PAIR_TWO:
            case SamCardAttackType.THREE_OFAKIND_TWO:
            case SamCardAttackType.FOUR_OFAKIND_TWO:
                return true;
        }
        return false;
    }

    public bool IsAttackTuQuy(List<int> cardIds)
    {
        if (cardIds.Count != 4)
            return false;

        cardIds = cardIds.OrderBy(a => a).ToList();

        int last = cardIds[0];
        for (int i = 1; i < cardIds.Count; i++)
        {
            if(cardIds[i] - last == 1)
            {
                last = cardIds[i];
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    public bool IsAttackTuQuy(SamCardAttackType type)
    {
        switch (type)
        {
            case SamCardAttackType.FOUR_OFAKIND_NORMAL:
            case SamCardAttackType.FOUR_OFAKIND_TWO:
                return true;
        }
        return false;
    }
}

[System.Serializable]
public class SRSSam
{
    public SRSSamGameSession session;
    public int moneyType;

    //public bool IsDealer()
    //{
    //    return session.Banker.Equals(myData.AccountId);
    //}

    //public bool IsPlayerAsDealer(string accountId)
    //{
    //    return session.Banker.Equals(accountId);
    //}



    public SRSSamGameStateItem GetGameStateUser(string accountId)
    {
        if(session.GameLoop.GameState.State.ContainsKey(accountId))
        {
            return session.GameLoop.GameState.State[accountId];
        }
        return null;
    }
}

[System.Serializable]
public class SRSSamAccount
{
    public string AccountID;
    public string UserName;
    public double Gold;
    public double Coin;
    public int Avatar;
}

[System.Serializable]
public class SRSSamPlayer
{
    public List<SRSSamCard> HandCards;
    public int OrderInGame; // thứ tự trong ván chơi(khác số Position: ví dụ có 2 người chơi ở Position 0 và 7, nhưng OrderInGame sẽ lần lượt là 0 và 1),
    public bool EmptyHand;
    public SRSSamAccount Account;
    public string AccountID;
    public string SessionID;
    public SamPlayerStatus Status;
    public int Position; // 0 -> 7 vị trí trong bàn
    public bool RegisterLeaveRoom;
    public string RemoteIP;
    public SamConnectionStatus ConnectionStatus;
    public string LastActiveTime; //thời gian cuối client của người chơi có kết nối lên server(dùng để kiểm tra và xóa các người chơi bị mất kết nối không rõ nguyên nhân),
}

[System.Serializable]
public class SRSSamTimer
{
    public double TotalTime;
    public double Time;
}

[System.Serializable]
public class SRSSamCard
{
    public bool Flip;
    public bool IsDraggable;
    public int CardNumber; //giá trị số của quân bài(từ 1..9),
    public SamSuite CardSuite; //giá trị chất của quân bài,
    public int OrdinalValue; //giá trị duy nhất của quân bài(tính từ -1, 0..51, với -1 là quân úp. OrdinalValue = (CardNumber - 3) * 4 + CardSuite / 13"
}

[System.Serializable]
public class SRSSamGameSession
{
    public SamRule Rule;
    public bool IsPlaying; // trạng thái đang chơi của ván(true: ván đang chơi, false, ván chưa bắt đầu),
    public bool IsComfirming;
    public string OwnerId; // accountId của người chơi làm Chương,
    public string LastWinner;
    public List<string> Positions; // mảng accountId của người chơi theo vị trí trong game(từ 0..7),
    public List<string> LeaveGameList; // danh sach nguoi choi dang ky out phong
    public int CountActivePlayer; //số người đang chơi trong ván,
    public int MaxAllow;
    public string Name; // tên của bàn chơi(phần số để chọn bàn),
    public int GameId;
    public int MaxPlayer;
    public int MinLevel;
    public int MaxLevel;
    public int MinBet; //tiền cược tối thiểu,
    public int MaxBet; //tiền cược tối đa,
    public bool IsPasswordProtected;
    public string RuleDescription;
    public bool IsPrivate;
    public SamMoneyType MoneyType;
    public int CurrentGameLoopId;
    public int CountPlayers; //số người chơi trong bàn chơi,
    public bool IsFull; // trạng thái bàn đã đầy(8 người),
    public bool IsEnough; // trạng thái bàn đủ số người để có thể chơi(2 người),
    public bool IsEmpty; 
    public bool IsAllow; 
    
    public Dictionary<string, SRSSamPlayer> Players;
    public SRSSamGameLoop GameLoop;
}

[System.Serializable]
public class SRSSamGameLoop
{
    public bool TimerPaused;
    public double Elapsed;
    public int Phrase;
    public List<SRSSamGameLoopTurn> CurrTurnCards; //(array<Key: long accountId, Value: { Count: int, Cards: int[] listCards }>) mảng các quân bài đã đánh trong các lượt đánh của vòng,
    public SRSSamSessionResult SessionResult;
    public SRSSamGameState GameState;
}

[System.Serializable]
public class SRSSamGameState
{
    public bool InBaoSamMode;
    public int Mode;
    //public List<string> BaoSamList;
    public string AccountBaoSam;
    public string AccountChanSam;
    public string DefaultAccount;
    public Dictionary<string, SRSSamGameStateItem> State;
}

[System.Serializable]
public class SRSSamGameStateItem
{
    public string AccountId;
    public string PrevAccountId;
    public SamActionName DefaultAction;
    public List<SamActionName> AllowedActions;
    public bool InTurn;
}

[System.Serializable]
public class SRSSamGameLoopTurn
{
    public string Key;
    public SRSSamGameLoopTurnItem Value;
}

[System.Serializable]
public class SRSSamGameLoopTurnItem
{
    public int Count;
    public List<int> Cards;
    public SamCardAttackType Type;
}

[System.Serializable]
public class SRSSamSessionResult
{
    public List<SRSSamResult> ResultList; //mảng kết quả trả về của từng người chơi trong ván
}

[System.Serializable]
public class SRSSamResult
{
    public string AccountId;
    public string Username;
    public SamResultFamily ResultFamily;
    public double Money; // số tiền thắng(+)/thua(-)
    public List<int> BaiThoi; //mảng giá trị các quân bài bị thối(nếu có)"
}

[System.Serializable]
public class SRSSamLobby
{
    public List<SRSSamLobbyItem> rooms;
    
    public List<SRSSamLobbyItem> GetRoomByMoneyType(SamMoneyType mType)
    {
        return rooms.Where(a => a.Item4 == (int)mType).OrderBy(a => a.Item1).ToList();
    }
}

[System.Serializable]
public class SRSSamLobbyItem
{
    public int Item1;
    public int Item2;
    public int Item3;
    public int Item4;
}
