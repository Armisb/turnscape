using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace GameAPI.Models;

public class Match
{
    public Guid Id {get; set;} = new Guid();
    
    [Required]
    public string State {get;set;}
    public Guid CurrentTurnPlayerId {get;set;}
    public Guid PlayerOneId {get;set;}
    public List<int> PlayerOneStats {get;set;}
    public Guid PlayerTwoId {get;set;}
    public List<int> PlayerTwoStats {get;set;}
    public bool IsFinished {get;set;}
}
