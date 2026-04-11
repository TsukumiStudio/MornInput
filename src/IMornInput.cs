using System;

namespace MornLib
{
    public interface IMornInput
    {
        string CurrentScheme { get; }
        IObservable<(string prev, string next)> OnSchemeChanged { get; }
        bool IsPressedAny(string actionName);
        bool IsPressedAll(string actionName);
        bool IsPerformed(string actionName);
        bool IsPressingAny(string actionName);
        bool IsPressingAll(string actionName);
        bool IsReleaseAny(string actionName);
        bool IsReleaseAll(string actionName);
        
        T ReadValue<T>(string actionName) where T : struct;
    }
}