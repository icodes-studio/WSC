![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/Assets/wsc.png)

*WebSocket Server+Client library*

*https://github.com/icodes-studio/WSC*


　

## # Environment

- **CLIENT**
    - *https://github.com/icodes-studio/WSC/tree/main/C*
    - **Developed & tested with**
        - Unity 2022.3.4f1
        - Unity 2021.3.29f1
        - Unity 2020.3.39f1
    - **Dependencies**
        - ***Newtonsoft.Json (3.2.1)***
            - *com.unity.nuget.newtonsoft-json*
            - *https://www.newtonsoft.com/json*
            - *https://github.com/JamesNK/Newtonsoft.Json*
        - ***NativeWebSocket***
            - *https://github.com/endel/NativeWebSocket*
            - *https://github.com/endel/NativeWebSocket.git#upm*
        - ***WebSocketSharp-netstandard (1.0.1)***
            - *https://github.com/sta/websocket-sharp*
            - *https://github.com/PingmanTools/websocket-sharp*
            - *https://www.nuget.org/packages/WebSocketSharp-netstandard*
        - ***UnityIngameDebugConsole***
            - *https://github.com/yasirkula/UnityIngameDebugConsole*
            - *https://assetstore.unity.com/packages/tools/gui/in-game-debug-console-68068*


　

- **SERVER**
    - *https://github.com/icodes-studio/WSC/tree/main/S*
    - **Developed & tested with**
        - Visual Studio 2022
        - S.exe: .NET 5.0, 6.0, 7.0
        - WSC.dll: .NET standard 2.1
    - **Dependencies**
        - *Newtonsoft.Json (13.0.3)*
            - *https://www.newtonsoft.com/json*
            - *https://github.com/JamesNK/Newtonsoft.Json*
            - *https://www.nuget.org/packages/Newtonsoft.Json*
        - *WebSocketSharp-netstandard (1.0.1)*
            - *https://github.com/sta/websocket-sharp*
            - *https://github.com/PingmanTools/websocket-sharp*
            - *https://www.nuget.org/packages/WebSocketSharp-netstandard*


　

　

## # Gettting started

- **SERVER BUILD & RUN**
    - Open the solution file with Visual Studio and build the entire solution.
        > *https://github.com/icodes-studio/WSC/blob/main/S/S.sln*
    - Run ***"S.exe"*** from the output path.
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/Assets/runserver.png)
    - The server has successfully started.
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/Assets/serverstarted.png)


　

- **CLIENT BUILD & RUN**
    - Open your client project in the Unity Editor.
        > *https://github.com/icodes-studio/WSC/tree/main/C*
    - Load the WSC demo scene.
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/Assets/unity-wsc.png)
    - Hit the play button, and it will exchange messages with the server.
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/Assets/unity-wsc-run.png)


　

　

## # Communicate with the AWS Chat service

- **PREPARATIONS**
    - [*Build Serverless Chat App with Amazon API Gateway*](https://github.com/icodes-studio/WSC/blob/main/A/README.md)


　

- **Enter your API Gateway IDs**
    - *source:* *https://github.com/icodes-studio/WSC/blob/main/C/Assets/WSC/DEMO/AWS/CHAT/ChatInfo.cs*
        ```csharp
        using System.Diagnostics;
        using UnityEngine;

        namespace AWS.CHAT
        {
            public static class ChatInfo
            {
                const string RestID = "YOUR-REST-ID";
                const string SocketID = "YOUR-SOCKET-ID";

                public static string NAME => SystemInfo.deviceName;
                public static string UID => Process.GetCurrentProcess().Id.ToString();
                public static string RID => Application.platform.ToString();
                public static string WSHost => $"wss://{SocketID}.execute-api.ap-northeast-2.amazonaws.com/dev/?userId={UID}&roomId={RID}";
                public static string W3Host => $"https://{RestID}.execute-api.ap-northeast-2.amazonaws.com/dev/chat/";
            }
        }
        ```
        > - RestID: Enter ***chatapp-rest's*** API ID
        > - SocketID: Enter ***chatapp-websocket's*** API ID
    - **You can check your API-Gateway-ID out in the AWS console.**
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/AWS/Chat%20(API%2BLambda%2BDynamoDB)/Assets/40.png)


　

- **CLIENT BUILD & RUN**
    - Open your client project in the Unity Editor.
        > *https://github.com/icodes-studio/WSC/tree/main/C*
    - Load the CHAT demo scene.
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/Assets/unity-wsc-chat.png)
    - Hit the play button, and it will exchange messages with the AWS Chat server.
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/Assets/unity-wsc-run.png)


　

　

## # Testing the RESTful API

- **SERVER BUILD & RUN**
    - Open the project ***S*** Properties and build with the ***W3C*** compilation symbol.
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/Assets/wsc-server-w3c.png)
    - Run ***"S.exe"*** from the output path.
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/Assets/runserver.png)
    - The server has successfully started.
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/Assets/serverstarted.png)


　

- **CLIENT BUILD & RUN**
    - Open your client project in the Unity Editor.
        > *https://github.com/icodes-studio/WSC/tree/main/C*
    - Enter the ***W3C*** scripting define symbol.
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/Assets/wsc-define-symbol.png)
    - Load the WSC demo scene.
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/Assets/unity-wsc.png)
    - Hit the play button, then you will see the RESTful WWW communications.
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/Assets/unity-rest-run.png)