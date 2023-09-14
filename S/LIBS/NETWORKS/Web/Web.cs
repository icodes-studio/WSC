using System;

namespace WSC
{
    public interface IWeb
    {
        void Query(RequestW3 request, Action<NetworkResponse> callback);
    }
}
