﻿namespace FiniteAutomatas.Domain.Models.ValueObjects;

public class Transition
{
    public StateId From { get; set; }
    public Argument Argument { get; }
    public StateId To { get; set; }

    public string? AdditionalData { get; }

    public Transition(
        StateId from,
        Argument argument,
        StateId to,
        string? additionalData = null )
    {
        From = from;
        Argument = argument;
        To = to;
        AdditionalData = additionalData;
    }

    public override bool Equals( object? obj )
    {
        return obj is Transition other && Equals( other );
    }

    public bool Equals( Transition other )
    {
        return From == other.From && To == other.To && Argument == other.Argument;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine( From.GetHashCode(), To.GetHashCode(), Argument.GetHashCode() );
    }

    public override string ToString()
    {
        return $"(From: {From}, To: {To}, Argument: {Argument})";
    }

    public static bool operator ==( Transition a, Transition b )
    {
        return a.Equals( b );
    }

    public static bool operator !=( Transition a, Transition b )
    {
        return !a.Equals( b );
    }
}