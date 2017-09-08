/* 
*┌──────────────────────────────────┐
*│　         Copyright(C) 2017 by Antiphon.All rights reserved.       │
*│                Author:by Locke Xie 2017-02-22  　　　　　　　　  　│
*└──────────────────────────────────┘
*
* 功 能： 功能方法
* 类 名： Method.cs
* 
* 修改历史：
* 
* 
*/

using System.Collections.Generic;

namespace MahjongMethod
{
    //静态方法
    public static class Method
    {
        /// <summary>
        /// 是否胡牌
        /// </summary>
        /// <param name="mah">手中的牌</param>
        /// <param name="ID">摸到或额外获得的牌</param>
        /// <returns></returns>
        public static bool IsCanHU(List<int> mah, int ID=0)
        {
            List<int> pais = new List<int>(mah);
            if (ID > 0)
            {
                pais.Add(ID);
            }

            //只有两张牌
            if (pais.Count == 2)
            {
                return pais[0] == pais[1];
            }
            if (IsQiDui(pais))
            {
                return true;
            }
            //先排序
            pais.Sort();
            //依据牌的顺序从左到右依次分出将牌
            for (int i = 0; i < pais.Count; i++)
            {
                List<int> paiT = new List<int>(pais);
                List<int> ds = pais.FindAll(delegate (int d)
                {
                    return pais[i] == d;
                });
                
                //判断是否能做将牌
                if (ds.Count >= 2)
                {
                    //移除两张将牌
                    paiT.Remove(pais[i]);
                    paiT.Remove(pais[i]);
                    //避免重复运算 将光标移到其他牌上
                    i += ds.Count;
                    if (HuPaiPanDin(paiT))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 排名能否组成顺子克子
        /// </summary>
        /// <param name="mahs"></param>
        /// <returns></returns>
        private static bool HuPaiPanDin(List<int> mahs)
        {
            if (mahs.Count == 0)
            {
                return true;
            }
            List<int> fs = mahs.FindAll(delegate (int a)
            {
                return mahs[0] == a;
            });

            //组成克子
            if (fs.Count == 3)
            {
                mahs.Remove(mahs[0]);
                mahs.Remove(mahs[0]);
                mahs.Remove(mahs[0]);
                return HuPaiPanDin(mahs);
            }
            else
            { //组成顺子
                if (mahs.Contains(mahs[0] + 1) && mahs.Contains(mahs[0] + 2))
                {
                    mahs.Remove(mahs[0] + 2);
                    mahs.Remove(mahs[0] + 1);
                    mahs.Remove(mahs[0]);
                    return HuPaiPanDin(mahs);
                }
                return false;
            }
        }

        /// <summary>
        /// 是否胡七对
        /// </summary>
        /// <param name="mahs"></param>
        /// <returns></returns>
        private static bool IsQiDui(List<int> mahs)
        {
            if (mahs.Count != GlobalData.PLAYER_CARD_NUM + 1)
            {
                return false;
            }
            for (int i = 0; i < mahs.Count/2; i++)
            {
                if (mahs[2 * i + 1] != mahs[2 * i + 2])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
