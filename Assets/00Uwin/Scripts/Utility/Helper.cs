using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper
{


    public static Dictionary<int, string> dictErrorCodes = new Dictionary<int, string>
    {
        {-1, "Mã xác nhận đã hết thời gian sử dụng!"},
        {-2, "Mã xác nhận không chính xác!"},
        {-10, "Thẻ này không được hệ thống chấp nhận"},
        {-11, "Bạn đã hết số lượt quay trong ngày"},
        {-13, "Thông tin thẻ không chính xác hoặc đã được sử dụng"},
        {-14,"Giao dịch không hợp lệ"},
        {-15, "Thông tin thẻ nạp không hợp lệ"},
        {-16, "Hệ thống đã ghi nhận thẻ, vui lòng đợi trong ít phút"},
        {-20, "Tên tài khoản không đúng quy định"},
        {-30, "Mật khẩu không đúng quy định"},
        {-31, "Mật khẩu cũ không chính xác"},
        {-40, "Tên hiển thị này đã tồn tại"},
        {-50, "Không xác nhận được tài khoản facebook này"},
        {-51, "Thông tin đăng nhập chưa đúng"},
        {-52, "Định dạng chứng minh thư chưa đúng"},
        {-53, "Định dạng email chưa đúng"},
        {-54, "Định dạng số điện thoại di động chưa đúng"},
        {-55, "Giftcode không hợp lệ hãy kiểm tra lại"},
        {-56, "Giftcode không chính xác hãy kiểm tra lại"},
        {-57, "Tên tài khoản đã tồn tại"},
        {-58, "Tên tài khoản không tồn tại"},
        {-60, "Mã OTP không chính xác, vui lòng kiểm tra lại!"},
        {-61, "Số dư không đủ để gửi két"},
        {-62, "Số dư không đủ"},
        {-65, "Tài khoản của bạn đã bị khóa"},
        {-70, "Chỉ có thể nhận mã OTP 5 phút một lần"},
        {-71, "Bạn cần đăng ký sử dụng App OTP"},
        {-72, "Thiết bị này không được cho phép đăng nhập App OTP"},
        {-80, "Số Vàng chuyển khoản tối thiểu là 10.000"},
        {-99, "Hệ thống đang bận, xin vui lòng thử lại sau."},
        {-100, "Loại thẻ này không chính xác"},
        {-101, " Xin vui lòng đợi hệ thống duyệt thẻ"},
        { 0,    "Xin vui lòng đợi trong ít phút, thẻ đang được duyệt"},
    };

    public static bool CheckResponseSuccess(int code, bool showPopup = true)
    {
        if (code == 1)
        {
            return true;
        }
        else
        {
            if (showPopup)
            {
                LPopup.OpenPopupError(code);
            }
            return false;
        }
    }

    public static bool CheckStatucSucess(WebServiceStatus.Status status)
    {
        if (status == WebServiceStatus.Status.OK)
        {
            return true;
        }

        LPopup.OpenPopupTop("Thống Báo!", "Vui lòng kiểm tra kết nối internet của bạn");

        return false;
    }

    public static string GetStringError(int code)
    {
        if(CodeReponseError.instance != null)
        {
            for(int i = 0; i < CodeReponseError.instance.listCodeBug.Count; i++)
            {
                if(code == CodeReponseError.instance.listCodeBug[i].id)
                {
                    return CodeReponseError.instance.listCodeBug[i].strNotice;
                }
            }
        }

        if (dictErrorCodes.ContainsKey(code))
        {
            return dictErrorCodes[code];
        }
        return "Hệ thống đang bận, xin vui lòng thử lại";
    }

    public static string ConvertStringToFormatTime(string data)
    {
        try
        {
            DateTime dateTime = DateTime.Parse(data);
            return dateTime.ToString("HH:mm dd/MM/yyyy");
        }
        catch
        {
            return "Ngày không xác định";
        }
    }
}
