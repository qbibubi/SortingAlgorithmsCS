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

            while(true)
            {
                while (list[low] < pivot)
                {
                    low++;
                }

                while (list[high] > pivot)
                {
                    high--;
                }

                if (low < high)
                {
                    if (list[low] == list[high])
                        return high;

                    int temp = list[low];
                    list[low] = list[high];
                    list[high] = temp;
                }
                else return high;

            }
        }

        public static List<int> Quicksort(List<int> list, int low, int high)
        {
            if (low < high)
            {
                int partitionIndex = Partition(list, low, high);

                //2. Recursively continue sorting the list
                if (partitionIndex > 1)
                    Quicksort(list, low, partitionIndex - 1);

                if (partitionIndex +1 < high)
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

        public static List<int> Merge(List<int> list, int p, int q, int r)
        {
            int n1 = q - p + 1;
            int n2 = r - q;
            int[] L = new int[n1];
            int[] R = new int[n2];
            int i, j, k;

            for (i = 0; i < n1; i++)
                L[i] = list[p + i];

            for (j = 0; j < n2; j++)
                R[j] = list[q + 1 + j];

            i = 0;
            j = 0;

            k = p;
            while (i < n1 && j < n2)
            {
                if (L[i] <= R[j])
                {
                    list[k] = L[i];
                    i++;
                }
                else
                {
                    list[k] = R[j];
                    j++;
                }
                k++;
            }

            while (i < n1)
            {
                list[k] = L[i];
                i++;
                k++;
            }

            while (j < n2)
            {
                list[k] = R[j];
                j++;
                k++;
            }

            return list;
        }

        public static List<int> Mergesort(List<int> list, int p, int r)
        {
            if (p < r)
            {
                int q = (p + r) / 2;

                Mergesort(list, p, q);
                Mergesort(list, q + 1, r);

                Merge(list, p, q, r);
            }

            return list;
        }
    }
}
