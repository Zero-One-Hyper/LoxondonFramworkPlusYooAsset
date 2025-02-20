using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LP.Framework
{
    public enum UNIT
    {
        BYTE,KB,MB,GB
    }
    public class Progress
    {
        private long totalSize = 0;
        private long completedSize = 0;
        
        private int totalCount = 0;
        private int completeCount = 0;

        private float speed = 0;
        private long lastTime = -1;
        private long lastValue = -1;
        private long lastTime2 = -1;
        private long lastValue2 = -1;

        public Progress() : this(0, 0)
        {
            
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="totalSize"></param>
        /// <param name="completedSize"></param>
        public Progress(long totalSize, long completedSize)
        {
            this.totalSize = totalSize;
            this.completedSize = completedSize;
            lastTime = DateTime.UtcNow.Ticks / 10000;
            lastValue = this.completedSize;

            lastTime2 = lastTime;
            lastValue2 = lastValue;
        }

        public long TotalSize
        {
            get { return this.totalSize; }
            set { this.totalSize = value; }
        }

        public long CompletedSize
        {
            get { return this.completedSize; }
            set
            {
                this.completedSize = value;
                //TODO update
                OnUpdate();
            }
        }

        public int TotalCount
        {
            get { return this.totalCount; }
            set { this.totalCount = value; }
        }

        public int CompletedCount
        {
            get { return this.completeCount; }
            set { this.completeCount = value; }
        }

        /// <summary>
        /// 更新
        /// </summary>
        private void OnUpdate()
        {
            long now = DateTime.UtcNow.Ticks / 10000;
            if ((now - lastTime) >= 1000)
            {
                lastTime2 = lastTime;
                lastValue2 = lastValue;

                this.lastTime = now;
                this.lastValue = this.completedSize;
            }

            float dt = (now - lastTime2) / 1000f;
            speed = (this.completedSize - this.lastValue2)/dt;
        }

        public virtual float Value
        {
            get
            {

                if (this.totalSize <= 0)
                    return 0f;
                return this.completedSize / (float)this.totalSize;
            }
        }

        public virtual float GetTotalSize(UNIT unit = UNIT.BYTE)
        {
            switch (unit)
            {
                case UNIT.KB:
                    return this.totalSize / 1024f;
                case UNIT.MB:
                    return this.totalSize / 1048576f;
                case UNIT.GB:
                    return this.totalSize/1073741824f;
                default:
                    return (float)this.totalSize;
            }
        }

        public virtual float GetCompletedSize(UNIT unit = UNIT.BYTE)
        {
            switch (unit)
            {
                case UNIT.KB:
                    return this.completedSize / 1024f;
                case UNIT.MB:
                    return this.completedSize/1048576f;
                case UNIT.GB:
                    return this.completedSize/1073741824f;
                default:
                    return (float)this.completedSize;
            }
        }

        public virtual float GetSpeed(UNIT unit = UNIT.BYTE)
        {
            switch (unit)
            {
                case UNIT.KB:
                    return this.speed / 1024f;
                case UNIT.MB:
                    return this.speed/1048576f;
                case UNIT.GB:
                    return this.speed/1073741824f;
                default:
                    return this.speed;
            }
        }
    }
    
}
