using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apriori
{
    class Apriori
    {

        /*40% of total number of items*/
        private const int minSupportCount = 13024;

        private static List<List<Object>> returnC1(ref List<String> Database)
        {
            List<List<Object>> itemSets = new List<List<Object>>();
            /*Minimum support count calculated by using minimum support to be 40%*/
            
            /*Scan all the database transactions*/
            foreach (String transaction in Database)
            {
                /*Get all the Items in transaction*/
                String[] transItems = transaction.Split(',');
                

                /*See for each transaction item, add in list if not present otherwise increment count, each transaction will have 14 item*/
                foreach (String itemName in transItems)
                {

                    string itemNameTemp = itemName.Trim();
                    Boolean itemInTable = false;

                    /*Search if itemset is already present in the list C1, than increase its count*/
                    for (int itemCount = 0; itemCount < itemSets.Count; itemCount++)
                    {
                        if (itemSets[itemCount][0].ToString() == itemNameTemp)
                        {
                            int countOfItemName = Convert.ToInt32(itemSets[itemCount][1]);
                            itemSets[itemCount][1] = ++countOfItemName;
                            itemInTable = true;
                        }

                    }

                    /*If item is not already present in list C1 and is found for first time, then add it in list with count 1*/
                    if (itemInTable == false)
                    {
                        List<Object> newItemSet = new List<Object>();
                        newItemSet.Add(itemNameTemp);
                        newItemSet.Add(1);

                        itemSets.Add(newItemSet);
                    }

                }
            }

            return itemSets;
        }

       /**A Check Function that two Rows Can be Joined*/
        private static Boolean checkJoinCondition(int numOfItemsPerRow, ref List<List<Object>> LkItemsList, int outerCounter, int innerCounter)
        {
            /*ValuesToMatch variable checks how many rows to check for join*/
            /*ValuesMatched variable will count how many items are same at same index*/
            int valuesToMatch = numOfItemsPerRow--,valuesMatched = 0;
            if (numOfItemsPerRow >= 0)
            {
                do
                {
                    /*This condition check for all if itemsets at same index match for join. If yes than increment it*/
                    if (LkItemsList[outerCounter][numOfItemsPerRow] == LkItemsList[innerCounter][numOfItemsPerRow]) 
                    {
                        valuesMatched++;                    
                    }
                    numOfItemsPerRow--;

                } while (numOfItemsPerRow >= 0);
            }
            else
                return true;

            /*If join condition is true retrn true */
            if(valuesMatched == valuesToMatch)
                return true;
            else
                return false;

           
        }
        /*Check function to see if itemset is present in transaction*/
        private static Boolean checkItemsInTrans(String transaction, ref List<List<Object>> LkItemsList, int outerCounter, int innerCounter)
        {
            int numOfIndexToJoin = LkItemsList[outerCounter].Count;
            int numOfItemsToMatch = 0;
            numOfIndexToJoin--;
            for (int count = 0; count < numOfIndexToJoin; count++)
            {
                if (count == (numOfIndexToJoin - 1))
                {
                    if (transaction.Contains(LkItemsList[innerCounter][count].ToString()) &&
                        transaction.Contains(LkItemsList[outerCounter][count].ToString()))
                    {
                        numOfItemsToMatch++;
                    }
                }
                else
                    if (transaction.Contains(LkItemsList[outerCounter][count].ToString()))
                    {
                        numOfItemsToMatch++;
                    }
            }
            if (numOfItemsToMatch == numOfIndexToJoin)
                return true;
            else
                return false;

        }

        /*Add itemset to Ck+1*/
        private static List<Object> addNewItemSets(ref List<List<Object>> LkItemsList, int outerCounter, int innerCounter)
        {
            /*Temporary list to hold itemsets after join in each iteration*/
            List<Object> CkItem = new List<object>();

            int numOfItemSetsToAdd = LkItemsList[outerCounter].Count;
            numOfItemSetsToAdd--;
            for (int count = 0; count < numOfItemSetsToAdd; count++)
            {
                if (count == (numOfItemSetsToAdd - 1))
                {
                    CkItem.Add(LkItemsList[outerCounter][count]);
                    CkItem.Add(LkItemsList[innerCounter][count]);
                }
                else
                {
                    CkItem.Add(LkItemsList[outerCounter][count]);
                }
            }

            return CkItem;
        }
        /******************************************/
        /********Generic Computation of Ck*********/
        ///******************************************/
        private static List<List<Object>> returnCk(ref List<String> Database, ref List<List<Object>> LkItemsList)
        {
            List<List<Object>> CkItemsList = new List<List<Object>>();
            for (int L1Count = 0; L1Count < LkItemsList.Count; L1Count++)
            {
                for (int L1Count2 = (L1Count + 1); L1Count2 < LkItemsList.Count; L1Count2++)
                {
                    /*Temporary list to hold itemsets after join in each iteration*/
                    List<Object> CkItem = new List<object>();
                    int CountofItems = 0, itemsPerRow = LkItemsList[L1Count].Count;

                    /*1st reduction for count value stored at last index, 2nd for exclusion of last value for checking*/
                    itemsPerRow = itemsPerRow - 2;                    
                    
                    if (checkJoinCondition(itemsPerRow, ref LkItemsList, L1Count,L1Count2))
                    {
                        /*If joint ItemSet is present in atleast one transaction together, then add it with its count*/
                        foreach (String transaction in Database)
                        {
                            if (checkItemsInTrans(transaction, ref LkItemsList, L1Count, L1Count2))
                            {
                                CountofItems++;
                            }
                        }

                        if (CountofItems > 0)
                        {
                            CkItem = addNewItemSets(ref LkItemsList, L1Count, L1Count2);
                            CkItem.Add(CountofItems);
                            CkItemsList.Add(CkItem);
                        }

                    }


                }
            }
            return CkItemsList;
        }
       
        /*This method generates itemsets which are frequent only in Ck after join has been performed*/
        /*It prunes all those elements which are nto freuqent. This is quite self explantory*/
        private static List<List<Object>> returnLK(ref List<List<Object>> Ck)
        {
            List<List<Object>> LkItems = new List<List<Object>>();
            for (int count = 0; count < Ck.Count; count++)
            {
                int lengthObject = Ck[count].Count;
                lengthObject--;
                int countOfItem = Convert.ToInt32(Ck[count][lengthObject]);
                if (countOfItem > minSupportCount)
                {
                    LkItems.Add(Ck[count]);
                }
            }
            return LkItems;
        }

       
        /*This is preprocessing method which simply concatenates the name of attribute before each value */
        /*and it discretize continous values such as age, capital gain etc.*/
        private static List<String> PreProcessing(string[] Database)
        {
            List<String> preProcessedDB = new List<String>();
            int lineNum = 0;
            foreach (String trans in Database)
            {
                String[] items = trans.Split(',');
                int index = 0;
                String newTrans = "";
                foreach (String item in items)
                {
                    String itemTemp = item.Trim();
                    if (itemTemp != "")
                    {
                        if (index == 0)
                        {
                            int age = Convert.ToInt32(itemTemp);
                            if (age > 0 && age <= 10)
                            {
                                newTrans = newTrans + "Age_1-10";
                            }
                            else if (age > 10 && age <= 20)
                            {
                                newTrans = newTrans + "Age_11-20";
                            }
                            else if (age > 20 && age <= 30)
                            {
                                newTrans = newTrans + "Age_21-30";
                            }
                            else if (age > 30 && age <= 40)
                            {
                                newTrans = newTrans + "Age_31-40";
                            }
                            else if (age > 40 && age <= 50)
                            {
                                newTrans = newTrans + "Age_41-50";
                            }
                            else if (age > 50 && age <= 60)
                            {
                                newTrans = newTrans + "Age_51-60";
                            }
                            else if (age > 60 && age <= 70)
                            {
                                newTrans = newTrans + "Age_61-70";
                            }
                            else if (age > 70 && age <= 80)
                            {
                                newTrans = newTrans + "Age_71-80";
                            }
                            else
                                newTrans = newTrans + "Age_>80";
                        }
                        else if (index == 1)
                        {
                            String workClass = itemTemp.ToString();
                            newTrans = newTrans + ", WorkClass_" + workClass;
                        }
                        else if (index == 2)
                        {
                            int fnlwgt = Convert.ToInt32(itemTemp);
                            if (fnlwgt > 0 && fnlwgt <= 50000)
                            {
                                newTrans = newTrans + ", FNLWGT_1-50000";
                            }
                            else if (fnlwgt > 50001 && fnlwgt <= 100000)
                            {
                                newTrans = newTrans + ", FNLWGT_500001-100000";
                            }
                            else if (fnlwgt > 100001 && fnlwgt <= 150000)
                            {
                                newTrans = newTrans + ", FNLWGT_100001-150000";
                            }
                            else if (fnlwgt > 150001 && fnlwgt <= 200000)
                            {
                                newTrans = newTrans + ", FNLWGT_150001-200000";
                            }
                            else if (fnlwgt > 200001 && fnlwgt <= 250000)
                            {
                                newTrans = newTrans + ", FNLWGT_200001-250000";
                            }
                            else if (fnlwgt > 250001 && fnlwgt <= 300000)
                            {
                                newTrans = newTrans + ", FNLWGT_250001-300000";
                            }
                            else
                                newTrans = newTrans + ", FNLWGT_>300000";
                        }
                        else if (index == 3)
                        {
                            String education = itemTemp.ToString();
                            newTrans = newTrans + ", education_" + education;
                        }
                        else if (index == 4)
                        {
                            int educationNum = Convert.ToInt32(itemTemp);
                            if (educationNum > 0 && educationNum <= 5)
                            {
                                newTrans = newTrans + ", educationNum_1-5";
                            }
                            else if (educationNum > 5 && educationNum <= 10)
                            {
                                newTrans = newTrans + ", educationNum_6-10";
                            }
                            else if (educationNum > 10 && educationNum <= 15)
                            {
                                newTrans = newTrans + ", educationNum_11-15";
                            }
                            else if (educationNum > 15 && educationNum <= 20)
                            {
                                newTrans = newTrans + ", educationNum_16-20";
                            }
                            else
                                newTrans = newTrans + ", educationNum_>20";

                        }
                        else if (index == 5)
                        {
                            String maritStatus = itemTemp.ToString();
                            newTrans = newTrans + ", MaritalStatus_" + maritStatus;
                        }
                        else if (index == 6)
                        {
                            String occupation = itemTemp.ToString();
                            newTrans = newTrans + ", Occupation_" + occupation;
                        }
                        else if (index == 7)
                        {
                            String relationship = itemTemp.ToString();
                            newTrans = newTrans + ", Relationship_" + relationship;
                        }
                        else if (index == 8)
                        {
                            String race = itemTemp.ToString();
                            newTrans = newTrans + ", Race_" + race;
                        }
                        else if (index == 9)
                        {
                            String sex = itemTemp.ToString();
                            newTrans = newTrans + ", Sex_" + sex;
                        }
                        else if (index == 10 || index == 11)
                        {
                            int capGainLoss = Convert.ToInt32(itemTemp);
                            if (capGainLoss >= 0 && capGainLoss <= 10000)
                            {
                                if (index == 10)
                                    newTrans = newTrans + ", CapitalGain_0-10000";
                                else if (index == 11)
                                    newTrans = newTrans + ", CapitalLoss_0-10000";
                            }
                            else if (capGainLoss > 10001 && capGainLoss <= 20000)
                            {
                                if (index == 10)
                                    newTrans = newTrans + ", CapitalGain_10001-20000";
                                else if (index == 11)
                                    newTrans = newTrans + ", CapitalLoss_10001-20000";

                            }
                            else if (capGainLoss > 20001 && capGainLoss <= 30000)
                            {
                                if (index == 10)
                                    newTrans = newTrans + ", CapitalGain_20001-30000";
                                else if (index == 11)
                                    newTrans = newTrans + ", CapitalLoss_20001-30000";
                            }
                            else if (capGainLoss > 30001 && capGainLoss <= 40000)
                            {
                                if (index == 10)
                                    newTrans = newTrans + ", CapitalGain_10001-20000";
                                else if (index == 11)
                                    newTrans = newTrans + ", CapitalLoss_10001-20000";
                            }
                            else if (capGainLoss > 40001 && capGainLoss <= 50000)
                            {
                                if (index == 10)
                                    newTrans = newTrans + ", CapitalGain_40001-50000";
                                else if (index == 11)
                                    newTrans = newTrans + ", CapitalLoss_40001-50000";
                            }
                            else
                                if (index == 10)
                                    newTrans = newTrans + ", CapitalGain_>50000";
                                else if (index == 11)
                                    newTrans = newTrans + ", CapitalLoss_>50000";

                        }
                        else if (index == 12)
                        {
                            int hoursPerWeek = Convert.ToInt32(itemTemp);
                            if (hoursPerWeek >= 0 && hoursPerWeek <= 10)
                            {
                                newTrans = newTrans + ", HoursPerWeek_0-10";
                            }
                            else if (hoursPerWeek > 10 && hoursPerWeek <= 20)
                            {
                                newTrans = newTrans + ", HoursPerWeek_11-20";
                            }
                            else if (hoursPerWeek > 30 && hoursPerWeek <= 40)
                            {
                                newTrans = newTrans + ", 3HoursPerWeek_1-40";
                            }
                            else if (hoursPerWeek > 40 && hoursPerWeek <= 50)
                            {
                                newTrans = newTrans + ", HoursPerWeek_41-50";
                            }
                            else if (hoursPerWeek > 50 && hoursPerWeek <= 60)
                            {
                                newTrans = newTrans + ", HoursPerWeek_51-60";
                            }
                            else if (hoursPerWeek > 60 && hoursPerWeek <= 70)
                            {
                                newTrans = newTrans + ", HoursPerWeek_61-70";
                            }
                            else if (hoursPerWeek > 70 && hoursPerWeek <= 80)
                            {
                                newTrans = newTrans + ", HoursPerWeek_71-80";
                            }
                            else if (hoursPerWeek > 80 && hoursPerWeek <= 90)
                            {
                                newTrans = newTrans + ", HoursPerWeek_81-90";
                            }
                            else
                                newTrans = newTrans + ", HoursPerWeek_>90";

                        }
                        else if (index == 13)
                        {
                            String country = itemTemp.ToString();
                            newTrans = newTrans + ", Country_" + country;
                        }
                        else
                            newTrans = newTrans + ", " + item;
                        index++;
                    }
                }
                lineNum++;

                preProcessedDB.Add(newTrans);
            }

            return preProcessedDB;
            
        }
        static void Main(string[] args)
        {
            string[] Database = System.IO.File.ReadAllLines(@"..\..\..\adult.txt");
            
            /*********************************/
            /*****Preprocessing on Data  *****/
            /*********************************/
            List<String> processedDatabase = new List<String>();

            /*processedDatabase variable has preprocessed database after discretization and attribute-value pair creation*/
            processedDatabase = PreProcessing(Database);

            /*Start the stopwatch to calculate time of execution of code*/
            System.Diagnostics.Stopwatch calcTime = System.Diagnostics.Stopwatch.StartNew();

            /*These are the variable CK and Lk which are used throught in algo. They have joined and then pruned element*/
            /*at each step respectively*/
            List<List<Object>> CkItemsList = new List<List<Object>>();
            List<List<Object>> LkItemsList = new List<List<Object>>();
            

            /*Initial C1 and L1 Computation peformed. C1 will have first count list of all items*/
            /*Lk will have pruned itemsets after checking for minimum support count. It will be L1*/
            CkItemsList = returnC1(ref processedDatabase);
            LkItemsList = returnLK(ref CkItemsList);

                        
            /*After first computation Ck and Lk itemsets will be generated for each step and loop will break when*/
            /*frequent itemset has been found or all frequent itemsets has been found. Freq Itemsets will be in Lk*/

            do
            {
                /*This is temporary Lk buffer, it will be used in case if at one point Ck has itemsets after join which are all*/
                /*below min supp count. In that case this temporary Lk buffer will hold values before that join and pruning will be*/
                /*applied on this to have the frequent most item or itemsets only*/
                List<List<Object>> LkItemsListPrevious = new List<List<Object>>();
                LkItemsListPrevious = LkItemsList;

                CkItemsList = returnCk(ref processedDatabase, ref LkItemsList);
                /*Lk*/
                LkItemsList = returnLK(ref CkItemsList);

                /*This if statement will be true when join of Ck resuted in itemsets all below min supp count, hence after pruning all*/
                /*itemsets are dropped. At this point LkItemsListPrevious the temporary buffer will be used*/
                if (LkItemsList.Count == 0)
                {
                    /*Apply pruning on temporary buffer LkItemsListPrevious and throw away all itemsets belwo minimum suppo count*/
                    LkItemsList = returnLK(ref LkItemsListPrevious);

                    /*This variable will be used if more than one itemsets are freuqent. This will keep count*/
                    int sameItemCount = 0;
                    int lastIndex = LkItemsList[0].Count - 1;

                    /*If after pruning there are more than one itemsets above min supp count, then sort items in a way so that */
                    /*frequent most itemset is at index 0, this loop will also check if more than one highest frequency count */
                    /*itemsets are found. If that is the case keep all those highest frequency itemsets with same supp count*/
                    if (LkItemsList.Count > 1)
                    {
                        /*Arrange all in Descending order and then 1st index with maximum count is frequent most itemset*/
                        for (int i = 0; i < LkItemsList.Count; i++)
                        {
                            for (int j = 0; j < LkItemsList.Count - 1; j++)
                            {
                                if (Convert.ToInt32(LkItemsList[j][lastIndex]) < Convert.ToInt32(LkItemsList[j + 1][lastIndex]))
                                {
                                    List<Object> tempObjList = new List<Object>();
                                    tempObjList = LkItemsList[j];
                                    LkItemsList[j] = LkItemsList[j + 1];
                                    LkItemsList[j + 1] = tempObjList;
                                }

                            }
                        }

                        /*Now all itemsets are sorted in previous step. Now check if there is more than one frequent itemset with same count*/
                        int lastIndexOfItemset = LkItemsList[sameItemCount].Count;
                        /*It has count of Itemsets*/
                        lastIndexOfItemset--;

                        /*This while loop only runs until there is at least one another frequent itemset, other than the one at index 0*/
                        while (LkItemsList[sameItemCount][lastIndexOfItemset] == LkItemsList[sameItemCount + 1][lastIndexOfItemset] && sameItemCount < (LkItemsList.Count - 1))
                        {
                            sameItemCount++;
                        }

                        /*Add all frequent itemset to  finalLk a temporary variable which writes back final result to LkItemsList*/
                        List<List<Object>> finalLk = new List<List<Object>>();
                        for (int i = 0; i <= sameItemCount; i++)
                        {
                            finalLk.Add(LkItemsList[i]);
                        }
                        LkItemsList = finalLk;
                    }
                }


            } while (LkItemsList.Count > 1 && CkItemsList.Count > 0); /*This loop will break if single freq itemset is found or join resulted*/
                                                                    /*in all non freq itemset and LkItemSet.Count == 0 condition become true*/


            /*Stop watch ends after algorithm completes*/
            Console.WriteLine("The Most Frequent Item Set is: \n");
            /*Print all frequent itemsets*/
            for (int i = 0; i < LkItemsList.Count; i++)
            {
                for (int j = 0; j < LkItemsList[i].Count-1; j++)
                {
                    Console.WriteLine(" "+LkItemsList[i][j]);
                }
            }

            /*Stop watch ends after algorithm completes*/
            Console.WriteLine("\nTotal Execution Time is  " + ((calcTime.ElapsedMilliseconds) / 1000) + " seconds \n");
                       
            if (System.Diagnostics.Debugger.IsAttached)
            {
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }; 
        }
    }
}
