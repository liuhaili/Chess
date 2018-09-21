using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessServer
{
    public class Utility
    {
        /// <summary>
        /// 随机排列数组元素
        /// </summary>
        /// <param name="myList"></param>
        /// <returns></returns>
        public static List<T> ListRandom<T>(List<T> myList)
        {
            Random ran = new Random();
            List<T> newList = new List<T>();
            int index = 0;
            T temp;
            for (int i = 0; i < myList.Count; i++)
            {
                index = ran.Next(0, myList.Count - 1);
                if (index != i)
                {
                    temp = myList[i];
                    myList[i] = myList[index];
                    myList[index] = temp;
                }
            }
            return myList;
        }
    }
}
