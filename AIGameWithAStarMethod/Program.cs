using System;
using System.Collections.Generic;
using System.Linq;


//The plan is
//1. For each parent search childs (add or remove water from target cup), evaluate them and add to the "ActualNodes" List.
//2. Select the Node with the least cost from the ActualNodes
//3. Repeat 1 and 2 points until find Node with amount = target
public class State
{
    public int Id { get; set; }
    public int Amount { get; set; }
    public int Cost { get; set; }
    public State? Parent { get; set; }
    public int Level { get; set; }
    public bool HasChilds { get; set; }
    public string Description { get; set; }

    public State(int id, int amout, int cost, int level, State? parent, string description)
    {
        Id = id;
        Amount = amout;
        Cost = cost;
        Level = level;
        Parent = parent;
        HasChilds = false;
        Description = description;
    }
}

public class WeightTaskSolver
{
    public static List<State> actualStates = new List<State>();
    public int Capacity1 { get; set; }
    public int Capacity2 { get; set; }
    public int TargetAmount { get; set; }

    static int Id = 0;

    public WeightTaskSolver(int capacity1, int capacity2, int targetAmount)
    {
        Capacity1= capacity1;
        Capacity2= capacity2;
        TargetAmount= targetAmount;
    }


    public void Solve()
    {       
        var initialState = new State(Id++, 0, 0, 0, null, "Initial cup:");
        actualStates.Add(initialState);
        var finishNode = SearchTargetAmount();
        while(finishNode == null) {
            var actualNode = GetTheLeastCostNode();
            GetNewChilds(actualNode);
            finishNode = SearchTargetAmount();
        }
        ShowTheSolution(finishNode);
    }
    
    public void GetNewChilds(State parent)
    {
        var currentLevel = parent.Level + 1;
        //var childAmounts = new List<int,string>
        //{
        //    //Adding water from capacity1
        //    parent.Amount + Capacity1,
        //    //Adding water from capacity2
        //    parent.Amount + Capacity2,
        //    //Removing water from capacity1
        //    parent.Amount - Capacity1,
        //    //Removing water from capacity2
        //    parent.Amount - Capacity2
        //};
        var childAmounts = new List<Tuple<int, string>>
        {
            Tuple.Create(parent.Amount + Capacity1, "Adding water from capacity1"),
            Tuple.Create(parent.Amount + Capacity2, "Adding water from capacity2"),
            Tuple.Create(parent.Amount - Capacity1, "Removing water to capacity1"),
            Tuple.Create(parent.Amount - Capacity2, "Removing water to capacity2")
        };

        foreach (var childAmount in childAmounts)
        {
            if (childAmount.Item1 > 0)
            {
                var cost = Math.Abs(TargetAmount - childAmount.Item1) + currentLevel;
                var existState = actualStates.FirstOrDefault(s=>s.Amount == childAmount.Item1);
                if(existState!=null)
                {
                    if(existState.Cost > cost)
                    {
                        actualStates.Remove(existState);
                        actualStates.Add(new State(Id++, childAmount.Item1, cost, currentLevel, parent, childAmount.Item2));
                    }                    
                }
                else
                {
                    actualStates.Add(new State(Id++, childAmount.Item1, cost, currentLevel, parent, childAmount.Item2));
                }
            }
        }
    }

    public State GetTheLeastCostNode()
    {
        var theLeastCostNode = actualStates.Where(s => !s.HasChilds).OrderBy(s => s.Cost).FirstOrDefault();
        if (theLeastCostNode != null)
        {
            theLeastCostNode.HasChilds = true;
            return theLeastCostNode;
        }
        else throw new Exception("No actual paths");        
    }

    public State? SearchTargetAmount()
    {
        return actualStates.FirstOrDefault(s => s.Amount == TargetAmount);
    }    

    public void ShowTheSolution(State node)
    {
        if (node.Parent != null)
        {
            ShowTheSolution(node.Parent);
        }
        Console.WriteLine(string.Format("{0}: {1}l", node.Description ,node.Amount.ToString()) );
    }

    //public void ShowNode(State node)
    //{

    //}
}

class Program
{
    static void Main()
    {
        int cap1 = 25;
        int cap2 = 8;
        int tamount = 3;

        var weightTaskSolver = new WeightTaskSolver(cap1, cap2, tamount);
        weightTaskSolver.Solve();
    }
}