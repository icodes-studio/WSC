# WSC

- *WebSocket Server+Client library*
- *https://github.com/icodes-studio/WSC*


　

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
    - Run S.exe from the output path.
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/Assets/runserver.png)
    - The server has successfully started.
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/Assets/serverstarted.png)


　

- **CLIENT BUILD & RUN**
    - Open your project in the Unity Editor.
        > *https://github.com/icodes-studio/WSC/tree/main/C*
    - Load the WSC demo scene.
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/Assets/unity-wsc.png)
    - Hit the play button, and it will exchange messages with the server.
        > ![](https://github.com/icodes-studio/wiki/blob/main/STUDY%2BRND/Assets/unity-wsc-run.png)


　

- [*Build Serverless Chat App with Amazon API Gateway*](https://github.com/icodes-studio/WSC/blob/main/A/README.md)