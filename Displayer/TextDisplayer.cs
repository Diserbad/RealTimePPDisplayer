﻿using System;
using System.IO;

namespace RealTimePPDisplayer.Displayer
{
    sealed class TextDisplayer : DisplayerBase
    {
        private readonly char[] _ppBuffer = new char[1024];
        private readonly char[] _hitBuffer = new char[1024];
        private int _ppStrLen;
        private int _hitStrLen;

        private readonly string[] _filenames=new string[2];

        private readonly bool _splited;

        public TextDisplayer(string filename,bool splited=false)
        {
            _splited = splited;

            if (!Path.IsPathRooted(filename))
                _filenames[0] = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);
            else
                _filenames[0] = filename;

            if (_splited)
            {
                string ext = Path.GetExtension(_filenames[0]);
                string path = Path.GetDirectoryName(_filenames[0]);
                string file = Path.GetFileNameWithoutExtension(_filenames[0]);
                _filenames[0] = $"{path}{Path.DirectorySeparatorChar}{file}-pp{ext}";
                _filenames[1] = $"{path}{Path.DirectorySeparatorChar}{file}-hit{ext}";
            }
            Clear();//Create File
        }

        public override void Clear()
        {
            base.Clear();
            using (File.Open(_filenames[0], FileMode.Create, FileAccess.Write, FileShare.Read))
                if(_splited)
                    using (File.Open(_filenames[1], FileMode.Create, FileAccess.Write, FileShare.Read)){}
        }
        private bool _init;

        public override void Display()
        {
            if (!_init)
            {
                foreach(var filename in _filenames)
                    if(filename!=null)
                        Sync.Tools.IO.CurrentIO.WriteColor(string.Format(DefaultLanguage.TEXT_MODE_OUTPUT_PATH_FORMAT, filename), ConsoleColor.DarkGreen);
                _init = true;
            }
            _ppStrLen = FormatPp().CopyTo(0,_ppBuffer,0);
            _hitStrLen = FormatHitCount().CopyTo(0, _hitBuffer, 0);

            StreamWriter[] streamWriters = new StreamWriter[2];

            if (_splited)
            {
                streamWriters[0] = new StreamWriter(File.Open(_filenames[0], FileMode.Create, FileAccess.Write, FileShare.Read));
                streamWriters[1] = new StreamWriter(File.Open(_filenames[1], FileMode.Create, FileAccess.Write, FileShare.Read));
            }
            else
            {
                streamWriters[0] = new StreamWriter(File.Open(_filenames[0], FileMode.Create, FileAccess.Write, FileShare.Read));
                streamWriters[1] = streamWriters[0];
            }

            streamWriters[0].Write(_ppBuffer, 0, _ppStrLen);
            if (!_splited)
                streamWriters[0].Write(Environment.NewLine);

            streamWriters[1].Write(_hitBuffer, 0, _hitStrLen);

            for (int i=0; i < _filenames.Length; i++)
                if(_filenames[i]!=null)
                    streamWriters[i].Dispose();
        }
    }
}
