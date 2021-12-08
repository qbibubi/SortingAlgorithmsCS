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

        static int Partition(List<int> list, int low, int high)
        {
            /* 1. Select a pivot point. */
            int pivot = list[high];
            int lowIndex = low - 1;

            /* 2. Reorder the collection. */
            for (int j = low; j < high; j++)
            {
                if (list[j] <= pivot)
                {
                    lowIndex++;

                    int temp = list[lowIndex];
                    list[lowIndex] = list[j];
                    list[j] = temp;
                }
            }

            int temp1 = list[lowIndex + 1];
            list[lowIndex + 1] = list[high];
            list[high] = temp1;

            return lowIndex + 1;
        }

        public static List<int> Quicksort(List<int> list, int low, int high)
        {
            if (low < high)
            {
                int partitionIndex = Partition(list, low, high);

                //3. Recursively continue sorting the array
                Quicksort(list, low, partitionIndex - 1);
                Quicksort(list, partitionIndex + 1, high);
            }

            return list;
        }

        public static List<int> Heapify(List<int> list, int n, int i)
        {
            int largest = i;
            int left = 2 * i + 1;
            int right = 2 * i + 2;

            if (left < n && list[left] > list[largest])
                largest = left;

            if (right < n && list[right] > list[largest])
                largest = right;

            if (largest != i)
            {
                int swap = list[i];
                list[i] = list[largest];
                list[largest] = swap;
                Heapify(list, n, largest);
            }

            return list;
        }

        public static List<int> Heapsort(List<int> list, int n)
        {
            for (int i = n / 2 - 1; i >= 0; i--)
                Heapify(list, n, i);

            for (int i = n - 1; i >= 0; i--)
            {
                int temp = list[0];
                list[0] = list[i];
                list[i] = temp;
                Heapify(list, i, 0);
            }

            return list;
        }
    }
}
