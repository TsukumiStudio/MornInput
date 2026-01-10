using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace MornLib
{
    public sealed class MornInputSequenceSolver : IPostTickable
    {
        private sealed class Sequence
        {
            public int Index { get; set; }
            public Key[] Keys { get; }
            public Action Callback { get; }

            public Sequence(Key[] keys, Action callback)
            {
                Keys = keys;
                Callback = callback;
                Index = 0;
            }
        }

        private readonly List<Sequence> _sequences = new();

        public void Register(Action callback, params Key[] keys)
        {
            if (keys == null || keys.Length == 0)
            {
                MornInputGlobal.LogError("キーの配列が空です。少なくとも1つのキーを指定してください。");
                return;
            }

            _sequences.Add(new Sequence(keys, callback));
        }

        public void PostTick()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return;
            }

            foreach (var sequence in _sequences)
            {
                Calculate(keyboard, sequence);
            }
        }

        private static void Calculate(Keyboard keyboard, Sequence sequence)
        {
            if (!keyboard.anyKey.wasPressedThisFrame)
            {
                return;
            }

            if (sequence.Index < sequence.Keys.Length && keyboard[sequence.Keys[sequence.Index]].wasPressedThisFrame)
            {
                sequence.Index++;
                if (sequence.Index == sequence.Keys.Length)
                {
                    sequence.Callback?.Invoke();
                    sequence.Index = 0;
                }
            }
            else
            {
                sequence.Index = 0;
            }
        }
    }
}