using System;
using System.Collections.Generic;
using System.Text;

namespace SortingAlgorithms
{
    public static class SortAlgs
    {
        public static List<int> BubbleSort(List<int> list)
        {
            int i, j, temp;
            bool swapped;

            for (i = 0; i < list.Count - 1; i++)
            {
                swapped = false;
                for (j = 0; j < list.Count - i - 1; j++)
                {
                    if (list[j] > list[j + 1])
                    {
                        // swap arr[j] and arr[j+1]
                        temp = list[j];
                        list[j] = list[j + 1];
                        list[j + 1] = temp;
                        swapped = true;
                    }
                }

                // IF no two elements were
                // swapped by inner loop, then break
                if (swapped == false)
                    break;
            }

            return list;
        }


        public static List<int> InsertionSort(List<int> list)
        {
            int i, key, j;
            for (i = 1; i < list.Count; i++)
            {
                key = list[i];
                j = i - 1;

                /* Move elements of arr[0..i-1], that are
                greater than key, to one position ahead
                of their current position */
                while (j >= 0 && list[j] > key)
                {
                    list[j + 1] = list[j];
                    j = j - 1;
                }
                list[j + 1] = key;
            }
                return list;
        }

    }
}
