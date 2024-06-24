using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Clones;

public class Node<T>
{
    public T Value { get; set; }
    public Node<T> Next { get; set; }

    public Node(T value)
    {
        Value = value;
    }
}

public class NodeStack<T>
{
    Node<T> head;
    int count;

    public bool IsEmpty
    {
        get { return count == 0; }
    }
    public int Count
    {
        get { return count; }
        set { count = value; }
    }

    public Node<T> Head
    {
        get { return head; }
        set { head = value; }
    }

    public void Push(T item)
    {
        var node = new Node<T>(item);
        node.Next = head;
        head = node;
        count++;
    }

    public T Pop()
    {
        if (IsEmpty)
            throw new InvalidOperationException("Стек пуст");
        Node<T> temp = head;
        head = head.Next;
        count--;
        return temp.Value;
    }

    public T Peek()
    {
        if (IsEmpty)
            throw new InvalidOperationException("Стек пуст");
        return head.Value;
    }
}
public class CloneStrain
{
    public readonly int Version;
    public NodeStack<string> Earned { get; set; }
    public NodeStack<string> Canceled { get; set; }

    public CloneStrain(GetID id)
    {
        Version = id.ID;

        Earned = new NodeStack<string>();
        Canceled = new NodeStack<string>();
    }
}

public class GetID
{
    private int id = 0;

    public int ID { get { return id++; } }
}

public class CloneVersionSystem : ICloneVersionSystem
{
    private readonly GetID Id;
    private readonly List<CloneStrain> Strains;

    public CloneVersionSystem()
    {
        this.Id = new GetID();
        this.Strains = new() { new CloneStrain(Id) };
    }

    void Learn(int ci, string pi)
    {
        Strains[ci].Earned.Push(pi);
        Strains[ci].Canceled = new NodeStack<string>();
    }

    void Rollback(int ci)
    {
        Strains[ci].Canceled.Push(Strains[ci].Earned.Pop());
    }

    void Relearn(int ci)
    {
        Strains[ci].Earned.Push(Strains[ci].Canceled.Pop());
    }

    void Clone(int ci)
    {
        Strains.Add(new CloneStrain(Id));
        Strains[Strains.Count - 1].Earned.Head = Strains[ci].Earned.Head;
        Strains[Strains.Count - 1].Earned.Count = Strains[ci].Earned.Count;
        Strains[Strains.Count - 1].Canceled.Head = Strains[ci].Canceled.Head;
        Strains[Strains.Count - 1].Canceled.Count = Strains[ci].Canceled.Count;
    }

    string Check(int ci)
    {
        if (Strains[ci].Earned.Count == 0)
            return "basic";
        else
            return Strains[ci].Earned.Peek();
    }

    public string Execute(string query)
    {
        var command = query.Split(' ');
        var ci = int.Parse(command[1]) - 1;
        switch (command[0])
        {
            case "learn":
                Learn(ci, command[2]);
                break;
            case "rollback":
                Rollback(ci);
                break;
            case "relearn":
                Relearn(ci);
                break;
            case "clone":
                Clone(ci);
                break;
            case "check":
                return Check(ci);
        }
        return null;
    }
}