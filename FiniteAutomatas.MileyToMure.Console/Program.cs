﻿using FiniteAutomatas.Domain.Convertors;
using FiniteAutomatas.Domain.Convertors.Convertors;
using FiniteAutomatas.Domain.Models.Automatas;
using FiniteAutomatas.Domain.Models.ValueObjects;
using FiniteAutomatas.Visualizations;

namespace FiniteAutomatas.MileyToMure.Console;

public class Program
{
    public static void Main()
    {
        Miley miley = BuildTestMiley()
            .PrintToConsole();

        FiniteAutomata dfa = miley
            .Convert( new MileyToMureConvertor() )
            .PrintToConsole()
            .PrintToImage( @"D:\Development\Projects\TestingStation\mure.png" );
    }

    private static Miley BuildTestMiley()
    {
        return new Miley( new[]
        {
            CreateTransition( "q1", "q4", "a", "1" ),
            CreateTransition( "q1", "q2", "b", "2" ),
            CreateTransition( "q1", "q5", "c", "1" ),

            CreateTransition( "q2", "q5", "a", "2" ),
            CreateTransition( "q2", "q1", "b", "1" ),
            CreateTransition( "q2", "q4", "c", "2" ),

            CreateTransition( "q3", "q3", "a", "2" ),
            CreateTransition( "q3", "q5", "b", "1" ),
            CreateTransition( "q3", "q4", "c", "2" ),

            CreateTransition( "q4", "q5", "a", "1" ),
            CreateTransition( "q4", "q8", "b", "2" ),
            CreateTransition( "q4", "q4", "c", "2" ),

            CreateTransition( "q5", "q7", "a", "1" ),
            CreateTransition( "q5", "q2", "b", "2" ),
            CreateTransition( "q5", "q1", "c", "1" ),

            CreateTransition( "q6", "q1", "a", "1" ),
            CreateTransition( "q6", "q3", "b", "2" ),
            CreateTransition( "q6", "q4", "c", "2" ),

            CreateTransition( "q7", "q5", "a", "1" ),
            CreateTransition( "q7", "q3", "b", "2" ),
            CreateTransition( "q7", "q7", "c", "2" ),

            CreateTransition( "q8", "q3", "a", "2" ),
            CreateTransition( "q8", "q5", "b", "1" ),
            CreateTransition( "q8", "q6", "c", "2" )
        } );
    }

    private static Transition CreateTransition(string from, string to, string arg, string output)
    {
        return new Transition(
            new State(from),
            to: new State(to),
            argument: new Argument(arg),
            additionalData: output);
    }
}