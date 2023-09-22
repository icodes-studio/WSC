## # Build Serverless Chat App with Amazon API Gateway, Lambda and DynamoDB
    ● AWS DynamoDB+Lambda+API Gateway 조합으로 Serverless 채팅 서버 구축.
    ● Backend 로직 Lambda는 NodeJS 사용.
    ● Frontend는 Unity3D로 채팅 클라이언트 제작.
    ● 이 문서는 AWS 서비스 구축을 중점으로 설명한다.
*https://youtu.be/SPtRdnB5wSE*


　

## # Solution overview and architecture

![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/Architectures.png)

- ***Features***
    - Unity3D로 만든 채팅 클라이언트를 프론트엔드로 구성
    - Backend 로직을 처리하기 위한 Lambda 구성
        - 채팅 불러오기
        - 채팅 입력, 저장 및 전달
        - 유저 목록에 추가
        - 유저 목록 삭제
    - REST API
        - 채팅 불러오기
        - 채팅 입력, 저장 및 웹소켓 전달
    - WebSocket API
        - 실시간 채팅 메시지 전달용
        - (소켓 연결시) 유저 목록에 추가
        - (소켓에 연결 해제시) 유저 목록 삭제
    - DynamoDB
        - REST API에 붙어서 채팅 내용을 저장하고 불러옴
        - WebSocket API에 붙어서 유저목록을 저장


　

## # API Gateway

*https://docs.aws.amazon.com/ko_kr/apigateway/latest/developerguide/welcome.html*

- ***What is it***
    - API 형식으로 AWS 서비스에 접근할 수 있도록 해주는 서비스
- ***Features***
    - REST API
        - Lambda를 HTTP 프로토콜 기반의 REST API로 호출
    - WebSocket
        - WebSocket 프로토콜로 Lambda를 호출
        - 각 접속 마다 ConnectionID가 부여된다
        - 이 ConnectionID를 DynamoDB에 저장해서 유저를 구분


　

## # Lambda

*https://docs.aws.amazon.com/ko_kr/lambda/latest/dg/welcome.html*

- ***What is it***
    - Serverless 기반으로 코드를 실행할 수 있는 서비스
- ***Features***    
    - 총 4개의 Lambda를 만들 예정
        - WebSocket 연결 
            - ConnectionID를 DynamoDB(유저목록)에 저장
        - WebSocket 연결 해제
            - ConnectionID를 DynamoDB(유저목록)에서 삭제
        - 채팅 입력
            - DynamoDB(채팅)에 채팅 내용 기록
            - 해당 방의 ConnectionID들을 DynamoDB에서 불러와 채팅 내용을 WebSocket을 통해 전달
        - 채팅 가져오기
            - DynamoDB(채팅)에서 채팅 내용을 가져오기


　

## # DynamoDB

*https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/Introduction.html*

- ***What is it***
    - Key-Value 기반의 완전관리 Document DB
- ***Features***
    - 총 2개의 테이블을 만들 예정
        - 채팅 메시지를 저장하기 위한 테이블
        - 유저목록을 저장하기 위한 테이블


　

## # Unity3D

*https://unity.com/*

- ***What is it***
    - 멀티 플랫폼 게임 엔진
- ***Features***
    - 간단한 채팅 UI를 갖춘 클라이언트 제작
    - WebSocket and HTTP 프로토콜로 API에 접근


　

　

## # Building steps

- ***Creating an IAM Role***
- ***Creating DynamoDB tables***
- ***Creating API Gateways***
- ***Creating Lambdas***
- ***Integrates with the API Gateways***
- ***Client test***


　

## # Creating an IAM Role

- ***What is it?***
    - *Identity and Acess Management*
    - 로직을 처리하는 Lambda에 붙어서 Lambda가 여러가지 AWS 서비스를 이용할 권한을 획득

　

- ***Steps***
    - ***IAM 대시보드 오픈***
        - *https://us-east-1.console.aws.amazon.com/iamv2/home?region=ap-northeast-2#/home*
    - ***"역할"*** > ***"역할 만들기"***
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/1.png)
    - ***"서비스 또는 사용 사례"*** > ***"Lambda"*** > ***"다음"***
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/2.png)
    - ***아래 권한 정책 연결***
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/3.png)
        > - ***AmazonAPIGatewayInvokeFullAccess***
        > - ***AmazonDynamoDBFullAccess***
        > - ***CloudWatchFullAccess***
    - ***"역할 이름 입력"*** > *chatapp-lambda-role* > ***"역할 생성"***
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/4.png)


　

　

## # Creating DynamoDB tables

- ***DynamoDB 콘솔 오픈***
    - *https://ap-northeast-2.console.aws.amazon.com/dynamodbv2/home?region=ap-northeast-2#tables*

　

- ***채팅 메시지를 저장하는 테이블 생성***
    - ***테이블 생성***
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/41.png)
    - ***"테이블 이름"*** > *chatapp-chat-messages* > ***"파티션 키"*** > *roomId(문자열)* > ***"정렬 키"*** > *timestamp(숫자)* > ***"테이블 생성"***
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/42.png)

　

- ***유저 목록을 저장하는 테이블 생성***
    - ***테이블 생성***
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/41.png)
    - ***"테이블 이름"*** > *chatapp-userlist* > ***"파티션 키"*** > *connectionId(문자열)* > ***"테이블 생성"***
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/43.png)
    - *테이블 목록에서* ***chatapp-userlist*** 선택
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/44.png)
    - ***"인덱스"*** > ***"인덱스 생성"***
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/45.png)
    - ***"인덱스 세부 정보"*** > ***"파티션 키"*** > *roomId(문자열)* > ***"정렬 키"*** > *userId(문자열)* > ***"인덱스 생성"***
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/46.png)


　

　

## # Creating API Gateways

- ***API Gateway 대시보드 오픈***
    - *https://ap-northeast-2.console.aws.amazon.com/apigateway/main/apis?region=ap-northeast-2*

　

- ***Creating a REST API***
    - ***"API 생성"***
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/7.png)
    - ***"REST API"*** > ***"구축"***
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/5.png)
    - ***"새 API"*** > ***"API 이름 입력"*** > *chatapp-rest* > ***"API 생성"***
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/6.png)

　

- ***Creating a WebSocket API***
    - ***"API 생성"***
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/7.png)
    - ***"WebSocket API"*** > ***"구축"***
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/8.png)
    - ***"API 이름 입력"*** > *chatapp-websocket* > ***"라우팅 선택 표현식 입력"*** > *request.body.action* > ***"다음"***
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/9.png)
    - ***"경로 추가"*** > *나중에 할 거임* > ***"다음"***
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/10.png)
    - ***"통합 연결"*** > *나중에 할 거임* > ***"다음"***
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/10.png)
    - ***"스테이지 추가"*** > ***"스테이지 이름"*** > *dev* > ***"다음"***
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/11.png)
    - ***"검토 및 생성"*** > ***"생성"***
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/12.png)


　

　

## # Creating Lambdas

- ***Lambda 함수 페이지 오픈***
    - *https://ap-northeast-2.console.aws.amazon.com/lambda/home?region=ap-northeast-2#/functions*

　

- ***채팅 메시지 목록을 가져오는 람다 만들기***
    - ***"함수"*** > ***"함수 생성"***
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/13.png)
    - ***"함수이름"*** > *chatapp-message-get* > ***"런타임"*** > *Node.js 14.x* > ***"기존 역할 사용"*** > *chatapp-lambda-role* > ***"함수 생성"***
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/14.png)
    - ***업로드 할 Backend 소스코드 준비하기***
        > - *https://github.com/icodes-studio/WSC/tree/main/A/Lambda*
    - ***압축(zip)*** > *source.zip*
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/15.png)
        >
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/16.png)
    - ***AWS Lambda 콘솔로 돌아가서*** > ***"에서 업로드"*** > ***".zip 파일"***
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/17.png)
    - ***업로드*** > *source.zip* >  ***"저장"***
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/18.png)
    - ***업로드 된 소스 코드 확인*** > *src/lambda/chat/get.js*
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/19.png)
    - ***"런타임 설정"*** > ***"편집"***
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/20.png)
    - ***"핸들러"*** > *src/lambda/chat/get.handler* > ***"저장"***
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/21.png)
    - ***같은 방식으로 아래 3개 함수와 핸들러를 더 만들자***
        - *chatapp-message-put* > *src/lambda/chat/put.handler*
        - *chatapp-onConnect* > *src/lambda/chat/onConnect.handler*
        - *chatapp-onDisconnect* > *src/lambda/chat/onDisconnect.handler*
    - ***마지막으로, chatapp-message-put 람다에서 사용하는 환경변수를 등록해야 한다***
    - ***put.js*** *코드에서* ***socket_api_gateway_id*** *환경 변수 사용하는 부분을 확인 할 수 있다*
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/22.png)
    - ***즉, Websocket API Gateway ID를 환경변수에 추가해야 한다.***
    - *API Gateway 콘솔로 돌아와서* > ***chatapp-websocket API의 ID 복사***
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/23.png)
    - ***람다 콘솔로 다시 돌아와서*** > *chatapp-message-put* > ***"구성"*** > ***"편집"***
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/24.png)
    - ***"환경 변수 추가"*** > ***"키"*** > *socket_api_gateway_id* > ***"값"*** > *붙여넣기* > ***"저장"***
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/25.png)


　

　

## # Integrates with the API Gateways

- ***API Gateway 콘솔 오픈***
    - *https://ap-northeast-2.console.aws.amazon.com/apigateway/main/apis?region=ap-northeast-2*

　

- ***WebSocket API 에 Lambda 연결***
    - ***chatapp-websocket*** API 선택
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/26.png)
    - ***"경로 생성"***
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/27.png)
    - ***"라우팅 키"*** > *$connect* > ***"Lambda 프록시 통합"*** > *on* > ***"Lambda 함수"*** > *chatapp-onConnect* > ***"경로 생성"***
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/28.png)
    - ***"경로 생성"*** > ***"라우팅 키"*** > *$disconnect* > ***"Lambda 프록시 통합"*** > *on* > ***"Lambda 함수"*** > *chatapp-onDisconnect* > ***"경로 생성"***
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/29.png)
    - ***"API 배포"***
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/30.png)
    - ***"배포 스테이지"*** > *dev* > ***"배포"***
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/31.png)
    - ***"스테이지 세부 정보 확인"***
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/47.png)

　

- ***REST API 에 Lambda 연결***
    - ***chatapp-rest*** 선택 
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/33.png)
    - ***"리소스 생성"***
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/32.png)
    - ***"리소스 이름"*** > *chat* > ***"CORS"*** > *on* > ***"리소스 생성"***
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/34.png)
    - ***"메서드 생성"***
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/35.png)
    - ***"메서드 유형"*** > *GET* > ***"Lambda 프록시 통합"*** > *on* > ***"Lambda 함수"*** > *chatapp-message-get* > ***"메서드 생성"***
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/36.png)
    - ***같은 방식으로 PUT 메소드 생성***
        - ***"메서드 유형"*** > *PUT* > ***"Lambda 프록시 통합"*** > *on* > ***"Lambda 함수"*** > *chatapp-message-put* > ***"메서드 생성"***
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/37.png)
    - ***API 배포*** > *새 스테이지* > ***"스테이지 이름"*** > *dev* > ***"배포"***
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/38.png)
        >
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/39.png)
    - ***"스테이지 세부 정보 확인"***
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/48.png)

　

　

## # Client test

- ***유니티 Frontend 준비하기***
    - *유니티 프로젝트:* *https://github.com/icodes-studio/WSC/tree/main/C*
    - ***API Gateway ID 소스 업데이트***
        - *업데이트할 소스:* *https://github.com/icodes-studio/WSC/blob/main/C/Assets/WSC/DEMO/AWS/CHAT/ChatInfo.cs*
        - *RestID:* ***chatapp-rest*** *API ID 입력*
        - *SocketID:* ***chatapp-websocket*** *API ID 입력*
        ```csharp
        using System.Diagnostics;
        using UnityEngine;

        namespace AWS.CHAT
        {
            public static class ChatInfo
            {
                const string RestID = "***"; // REST API ID
                const string SocketID = "***"; // WEBSOCKET API ID

                public static string NAME => SystemInfo.deviceName;
                public static string UID => Process.GetCurrentProcess().Id.ToString();
                public static string RID => Application.platform.ToString();
                public static string WSHost => $"wss://{SocketID}.execute-api.ap-northeast-2.amazonaws.com/dev/?userId={UID}&roomId={RID}";
                public static string W3Host => $"https://{RestID}.execute-api.ap-northeast-2.amazonaws.com/dev/chat/";
            }
        }
        ```
    - ***API Gateway ID는 콘솔에서 확인 할 수 있다.***
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/40.png)

　

- ***Code Review***
    - *https://github.com/icodes-studio/WSC/blob/main/C/Assets/WSC/DEMO/AWS/CHAT/Chat.cs*
        ```csharp
        using UnityEngine;
        using WSC;

        namespace AWS.CHAT
        {
            public class Chat : MonoBehaviour
            {
                private void Awake()
                {
                    Log.Initialize(string.Empty, string.Empty, LogLevel.Debug, (level, message) =>
                    {
                        switch (level)
                        {
                            case LogLevel.Debug:
                            case LogLevel.Information:
                            case LogLevel.Message:
                                Debug.Log(message);
                                break;
                            case LogLevel.Warning:
                                Debug.LogWarning(message);
                                break;
                            case LogLevel.Error:
                                Debug.LogError(message);
                                break;
                            case LogLevel.Fatal:
                                Debug.LogAssertion(message);
                                break;
                        }
                    });

                    // 네트워크 라이브러리 준비
                    WSC.Network.Initialize(new WSC.DEMO.WebSocketFactory());
                }

                private void Start()
                {
                    // 웹소켓 접속
                    NetworkWSClient.i.Connect(ChatInfo.WSHost).OnOpen += (response) =>
                    {
                        Log.Debug($"Socket Connected: {response.Data}");

                        // 채팅 목록 가져오기 via REST call
                        new RequestChatHistory().Query<AnswerChatHistory>((answer) =>
                        {
                            // 채팅 목록 출력
                            foreach (var message in answer.messages)
                                Log.Debug($"roomId:{message.roomId}, userId:{message.userId}, message:{message.message}, timestamp:{message.timestamp}");

                            // 채팅 메시지 전송하기 via REST call
                            new RequestChatMessage() { text = "Hello World" }.Query<AnswerChatMessage>((answer) =>
                                Log.Debug("Transfer completed."));
                        });
                    };

                    // 채팅 메시지 입력 실시간 통지 핸들러 via Websocket
                    NotifyChatMessage.OnNotify += (notify) =>
                    {
                        Log.Debug($"roomId:{notify.roomId}, userId:{notify.userId}, name:{notify.name}, message:{notify.message}, timestamp:{notify.timestamp}");
                    };

                    // 웹소켓 종료 핸들러
                    NetworkWSClient.i.OnClose += (response) =>
                    {
                        Log.Debug($"Socket Closed: {response.Data}");
                    };
                }
            }
        }
        ```