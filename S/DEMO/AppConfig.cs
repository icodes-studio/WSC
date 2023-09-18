using WSC;

namespace DEMO
{
    record AppConfig(
        string Environment,
        string LogPath,
        LogLevel LogLevel,
        string Service,
        int Port)
    {
    }
}
