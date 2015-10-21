﻿using UnityEngine;
using System.Collections.Generic;
using com.spacepuppy.Collections;
using com.spacepuppy.Utils;
using System;

namespace com.spacepuppy.Scenario
{

    [System.Serializable()]
    public class Trigger : ICollection<TriggerTarget>
    {

        #region Fields

        [SerializeField()]
        private bool _yield;

        [SerializeField()]
        private List<TriggerTarget> _targets = new List<TriggerTarget>();

        [System.NonSerialized()]
        private System.Action<object> _handlers;

        [System.NonSerialized()]
        private IObservableTrigger _owner;
        [System.NonSerialized()]
        private string _id;
        
        #endregion

        #region CONSTRUCTOR

        public Trigger()
        {

        }

        public Trigger(bool yielding)
        {
            _yield = yielding;
        }

        #endregion

        #region Properties

        public bool Yielding
        {
            get { return _yield; }
            set { _yield = value; }
        }

        /// <summary>
        /// The INotificationDispatcher associated with this trigger that will post the notification of it being triggered.
        /// </summary>
        public IObservableTrigger ObservableTriggerOwner
        {
            get { return _owner; }
            set { _owner = value; }
        }

        public string ObservableTriggerId
        {
            get { return _id; }
            set { _id = value; }
        }

        public List<TriggerTarget> Targets
        {
            get { return _targets; }
        }

        #endregion

        #region Methods

        public void AddHandler(System.Action<object> handler)
        {
            _handlers += handler;
        }

        public void RemoveHandler(System.Action<object> handler)
        {
            _handlers -= handler;
        }



        public TriggerTarget AddNew()
        {
            var targ = new TriggerTarget();
            _targets.Add(targ);
            return targ;
        }

        public void ActivateTrigger()
        {
            if (_targets.Count > 0)
            {
                var e = _targets.GetEnumerator();
                while (e.MoveNext())
                {
                    if (e.Current != null) e.Current.Trigger();
                }
            }

            if (_handlers != null)
                _handlers(null);

            //if(_owner != null)
            //{
            //    _owner.PostNotification<TriggerActivatedNotification>(new TriggerActivatedNotification(_owner, _id), false);
            //}
        }

        public void ActivateTrigger(object arg)
        {
            if (_targets.Count > 0)
            {
                var e = _targets.GetEnumerator();
                while (e.MoveNext())
                {
                    if (e.Current != null) e.Current.Trigger(arg);
                }
            }

            if (_handlers != null)
                _handlers(arg);

            //if (_owner != null)
            //{
            //    _owner.PostNotification<TriggerActivatedNotification>(new TriggerActivatedNotification(_owner, _id), false);
            //}
        }
        
        public void ActivateRandomTrigger(bool considerWeights)
        {
            if (_targets.Count > 0)
            {
                TriggerTarget trig = (considerWeights) ? _targets.PickRandom((t) => { return t.Weight; }) : _targets.PickRandom();
                trig.Trigger();
            }

            if (_handlers != null)
                _handlers(null);
            
            //if (_owner != null)
            //{
            //    _owner.PostNotification<TriggerActivatedNotification>(new TriggerActivatedNotification(_owner, _id), false);
            //}
        }

        public void ActivateRandomTrigger(object arg, bool considerWeights)
        {
            if (_targets.Count > 0)
            {
                TriggerTarget trig = (considerWeights) ? _targets.PickRandom((t) => { return t.Weight; }) : _targets.PickRandom();
                trig.Trigger();
            }

            if (_handlers != null)
                _handlers(arg);

            //if (_owner != null)
            //{
            //    _owner.PostNotification<TriggerActivatedNotification>(new TriggerActivatedNotification(_owner, _id), false);
            //}
        }

        public IRadicalYieldInstruction ActivateTriggerYielding()
        {
            if (_yield && _targets.Count > 0)
            {
                var instruction = BlockingTriggerYieldInstruction.Create();

                var e = _targets.GetEnumerator();
                while (e.MoveNext())
                {
                    if (e.Current != null) e.Current.Trigger();
                }

                if (_handlers != null)
                    _handlers(null);

                return (instruction.Count > 0) ? instruction : null;
            }
            else
            {

                if (_targets.Count > 0)
                {
                    var e = _targets.GetEnumerator();
                    while (e.MoveNext())
                    {
                        if (e.Current != null) e.Current.Trigger();
                    }
                }

                if (_handlers != null)
                    _handlers(null);

                return null;
            }
        }

        public IRadicalYieldInstruction ActivateTriggerYielding(object arg)
        {
            if (_yield && _targets.Count > 0)
            {
                var instruction = BlockingTriggerYieldInstruction.Create();

                var e = _targets.GetEnumerator();
                while (e.MoveNext())
                {
                    if (e.Current != null) e.Current.Trigger(arg);
                }

                if (_handlers != null)
                    _handlers(arg);

                return (instruction.Count > 0) ? instruction : null;
            }
            else
            {
                if (_targets.Count > 0)
                {
                    var e = _targets.GetEnumerator();
                    while (e.MoveNext())
                    {
                        if (e.Current != null) e.Current.Trigger(arg);
                    }
                }

                if (_handlers != null)
                    _handlers(arg);

                return null;
            }
        }


        public void DaisyChainTriggerYielding(object arg, BlockingTriggerYieldInstruction instruction)
        {
            if (_targets.Count > 0)
            {
                var e = _targets.GetEnumerator();
                while (e.MoveNext())
                {
                    if (e.Current != null) e.Current.Trigger(arg);
                }

                if (_handlers != null)
                    _handlers(arg);
            }
        }

        #endregion

        #region ICollection Interface

        public void Add(TriggerTarget item)
        {
            _targets.Add(item);
        }

        public void Clear()
        {
            _targets.Clear();
        }

        public bool Contains(TriggerTarget item)
        {
            return _targets.Contains(item);
        }

        public void CopyTo(TriggerTarget[] array, int arrayIndex)
        {
            _targets.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                if (_handlers == null)
                    return _targets.Count;
                else
                    return _targets.Count + 1;
            }
        }

        bool ICollection<TriggerTarget>.IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(TriggerTarget item)
        {
            return _targets.Remove(item);
        }

        public Enumerator GetEnumerator()
        {
            //return _targets.GetEnumerator();
            return new Enumerator(this);
        }

        IEnumerator<TriggerTarget> IEnumerable<TriggerTarget>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion


        #region Special Types

        public class ConfigAttribute : System.Attribute
        {
            public bool Weighted;
            public bool AlwaysExpanded;

            public ConfigAttribute()
            {

            }

        }

        public struct Enumerator : IEnumerator<TriggerTarget>
        {

            private List<TriggerTarget>.Enumerator _e;

            public Enumerator(Trigger t)
            {
                _e = t._targets.GetEnumerator();
            }

            public TriggerTarget Current
            {
                get { return _e.Current; }
            }

            object System.Collections.IEnumerator.Current
            {
                get { return _e.Current; }
            }

            public bool MoveNext()
            {
                return _e.MoveNext();
            }

            void System.Collections.IEnumerator.Reset()
            {
                (_e as IEnumerator<TriggerTarget>).Reset();
            }

            public void Dispose()
            {
                _e.Dispose();
            }

        }

        #endregion

    }
    
    public class BlockingTriggerYieldInstruction : RadicalYieldInstruction, IPooledYieldInstruction
    {

        #region Fields

        private int _count;

        #endregion

        #region CONSTRUCTOR

        private BlockingTriggerYieldInstruction()
        {

        }

        #endregion

        #region Properties

        public int Count
        {
            get { return _count; }
        }

        #endregion

        #region Methods

        public void BeginBlock()
        {
            _count++;
        }

        public void EndBlock()
        {
            if (this.IsComplete) return;

            _count--;
            if(_count <= 0)
            {
                this.SetSignal();
            }
        }

        #endregion

        #region IDisposable Interface

        void IDisposable.Dispose()
        {
            _count = 0;
            _pool.Release(this);
        }

        #endregion

        #region Static Factory

        private static ObjectCachePool<BlockingTriggerYieldInstruction> _pool = new ObjectCachePool<BlockingTriggerYieldInstruction>(-1, () => new BlockingTriggerYieldInstruction());

        public static BlockingTriggerYieldInstruction Create()
        {
            var obj = _pool.GetInstance();
            obj._count = 0;
            return obj;
        }

        #endregion

    }

}
