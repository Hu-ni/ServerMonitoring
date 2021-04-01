using Newtonsoft.Json.Linq;
using RestSharp;
using ServerMonitoring.Constants;
using ServerMonitoring.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ServerMonitoring.Services
{
    public interface IKakaoApiService
    {

    }

    public class KakaoApiService
    {
        private KakaoData data;
        public KakaoData Data { get { return data; } set { data = value; } }

        public KakaoApiService(KakaoData data)
        {
            this.data = data ?? throw new ArgumentNullException(nameof(data));
        }

        public bool GetUserToKen(WebBrowser webBrowser)
        {
            string wUrl = webBrowser.Source.ToString();
            string userToken = wUrl.Substring(wUrl.IndexOf("=") + 1);

            if (wUrl.CompareTo(KakaoKey.KakaoRedirectUrl + "?code=" + userToken) == 0)
            {
                Console.WriteLine(userToken);
                Console.WriteLine("유저 토큰 얻기 성공");
                data.userToken = userToken;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool GetAccessToKen()
        {
            var client = new RestClient(KakaoKey.KakaoHostOAuthUrl);

            var request = new RestRequest(KakaoKey.KakaoOAuthUrl, Method.POST);
            request.AddParameter("grant_type", "authorization_code");
            request.AddParameter("client_id", KakaoKey.KakaoRestApiKey);
            request.AddParameter("redirect_uri", KakaoKey.KakaoRedirectUrl);
            request.AddParameter("code", data.userToken);

            var restResponse = client.Execute(request);
            Console.WriteLine(restResponse.Content);
            var json = JObject.Parse(restResponse.Content);

            data.accessToken = json["access_token"].ToString();

            return true;
        }

        public void KakaoTalkLogOut()
        {
            var client = new RestClient(KakaoKey.KakaoHostApiUrl);

            var request = new RestRequest(KakaoKey.KakaoUnlinkUrl, Method.POST);
            request.AddHeader("Authorization", "bearer " + data.accessToken);

            if (client.Execute(request).IsSuccessful)
            {
                Console.WriteLine("로그아웃 성공");
            }
            else
            {
                Console.WriteLine("로그아웃 실패");
            }
        }


        /// <summary>
        /// 커스텀 메시지 보내기
        /// </summary>
        /// <param name="sendMessageObject">템플릿 JObject 값</param>
        public IRestResponse KakaoDefaultSendMessage(JObject sendMessageObject)
        {
            var client = new RestClient(KakaoKey.KakaoHostApiUrl);

            var request = new RestRequest(KakaoKey.kakaoDefaultFriendMessageUrl, Method.POST);
            request.AddHeader("Authorization", "bearer " + data.accessToken);
            request.AddParameter("template_object", sendMessageObject.ToString());
            request.AddParameter("talk_message", "test");


            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                Console.WriteLine(response.StatusCode);
                Console.WriteLine("메시지 보내기 성공");
            }
            else
            {
                Console.WriteLine(response.Content);
                Console.WriteLine("메시지 보내기 실패");
            }
            return response;
        }

        public string KakaoLoadFriendList()
        {
            var client = new RestClient(KakaoKey.KakaoHostApiUrl);

            var request = new RestRequest(KakaoKey.kakaoFrinedUrl, Method.POST);
            request.AddHeader("Authorization", "bearer " + data.accessToken);

            var restResponse = client.Execute(request);
            Console.WriteLine(restResponse.Content);
            return null;
        }

        /// <summary>
        /// 친구에게 커스텀 메시지 보내기
        /// </summary>
        /// <param name="sendMessageObject">템플릿 JObject 값</param>
        public string KakaoDefaultSendMessageForFreind(JObject sendMessageObject)
        {
            var client = new RestClient(KakaoKey.KakaoHostApiUrl);

            var request = new RestRequest(KakaoKey.KakaoDefaultMessageUrl, Method.POST);
            request.AddHeader("Authorization", "bearer " + data.accessToken);
            request.AddParameter("template_object", sendMessageObject.ToString());
            request.AddParameter("talk_message", "test");


            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                Console.WriteLine("메시지 보내기 성공");
            }
            else
            {
                Console.WriteLine(response.Content);
                Console.WriteLine("메시지 보내기 실패");
            }
            return response.Content;
        }
        public void KakaoUserData()
        {
            var client = new RestClient(KakaoKey.KakaoHostApiUrl);

            var request = new RestRequest(KakaoKey.KakaoUserDataUrl, Method.GET);
            request.AddHeader("Authorization", "bearer " + data.accessToken);

            var restResponse = client.Execute(request);
            var json = JObject.Parse(restResponse.Content);

            // 프로필 사진이 없을 경우 예외 처리
            if (json["properties"]["profile_image"] != null)
            {
                string UserImgUrl = json["properties"]["profile_image"].ToString();
                data.UserImg = WebImageView(UserImgUrl);
            }

            data.UserNickName = json["properties"]["nickname"].ToString();

            Console.WriteLine(json);
        }

        // 웹 이미지 다운로드
        private static Bitmap WebImageView(string url)
        {
            try
            {
                using (WebClient Downloader = new WebClient())
                using (Stream ImageStream = Downloader.OpenRead(url))
                {
                    Bitmap DownloadImage = Bitmap.FromStream(ImageStream) as Bitmap;
                    Console.WriteLine("이미지 다운 성공");
                    return DownloadImage;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("이미지 다운 실패");
                return null;
            }
        }
    }
}
