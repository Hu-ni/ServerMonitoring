using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerMonitoring.Models
{
    //프로그램을 사용하기 위한 사용자 정보
    public class KakaoData
    {
        // 유저 데이터
        public string userToken; // 유저 토큰
        public string accessToken; // 에셋 토큰
        public string UserNickName; // 사용자 이름
        public Bitmap UserImg; // 사용자 이미지
    }
}
