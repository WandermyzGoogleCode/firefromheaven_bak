using System;
using System.Collections.Generic;
using System.Text;
using FireEngine.FireMLEngine;
using FireEngine.FireMLEngine.Runtime;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using FireEngine.FireMLEngine.AST;

namespace FireEngine.Test.ConsoleRuntimeEnv
{
    class Program
    {
        static void Main(string[] args)
        {
            string filepath = args[0];
            FileStream rootStream = new FileStream(filepath, FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();
            FireMLRoot root = bf.Deserialize(rootStream) as FireMLRoot;

            FuncCaller funcCaller = new FuncCaller();
            RuntimeKernel kernel = new RuntimeKernel(funcCaller, root);

            do
            {
                kernel.Next();
                Console.ReadKey(true);
            } while (!funcCaller.IsEnd);
        }
    }

    class FuncCaller : IEngineFunctionCaller
    {
        bool isEnd = false;
        public bool IsEnd { get { return isEnd; } }

        #region IEngineFunctionCaller Members

        public FuncReturnBehavior ActionLayerDef(string name, FireEngine.FireMLData.PositionData position)
        {
            return FuncReturnBehavior.Next;
        }

        public FuncReturnBehavior Actor(string name, string img, string asset, string avatar, string avaasset, string layer, FireEngine.FireMLData.PositionData position)
        {
            Console.WriteLine("Actor: " + name + (img != null ? (", " + img) : ""));
            return FuncReturnBehavior.Next;
        }

        public FuncReturnBehavior Background(string img, string asset)
        {
            Console.WriteLine("Bg" + (img != null ? (", " + img) : ""));
            return FuncReturnBehavior.Next;
        }

        public FuncReturnBehavior Dialog(string text)
        {
            Console.WriteLine("Dialog: " + text);
            return FuncReturnBehavior.WaitForUser;
        }

        public FuncReturnBehavior Echo(string result)
        {
            Console.WriteLine("Echo: " + result);
            return FuncReturnBehavior.WaitForUser;
        }

        public FuncReturnBehavior Music(string src, string asset, bool loop, TimeSpan fadeIn)
        {
            Console.WriteLine("Music");
            return FuncReturnBehavior.Next;
        }

        public FuncReturnBehavior MusicStop(TimeSpan fadeOut)
        {
            Console.WriteLine("MusicStop");
            return FuncReturnBehavior.Next;
        }

        public FuncReturnBehavior MusicVol(double amplitude, TimeSpan transitionTime)
        {
            Console.WriteLine("MusicVol");
            return FuncReturnBehavior.Next;
        }

        public FuncReturnBehavior Select(string varName, SelectOption[] options)
        {
            Console.WriteLine("Select：");
            int counter = 1;
            foreach (SelectOption option in options)
            {
                Console.WriteLine("\t{0}: {1}", counter, option.Text);
                counter++;
            }

            bool success = false;
            int result = 0;

            do
            {
                Console.Write("请输入选项前的数字并按回车：");
                string input = Console.ReadLine();
                if (!int.TryParse(input, out result))
                {
                    Console.WriteLine("请输入一个数字！");
                    continue;
                }
                else if (result <= 0 || result > options.Length)
                {
                    Console.WriteLine("没有这个选项！");
                    continue;
                }
                else
                {
                    success = true;
                }
            } while (!success);

            if (UserSelected != null)
            {
                UserSelected(this, new SelectEventArgs(varName, options[result - 1].Value));
            }

            return FuncReturnBehavior.Next;
        }

        public void IssueWarning(string message)
        {
            Console.WriteLine("Warning: " + message);
        }

        public void End()
        {
            Console.WriteLine("FireML End!");
            isEnd = true;
        }

        public event SelectEventHandler UserSelected;

        #endregion
    }

}
