﻿#region

using System.Linq.Expressions;
using Backend.Entities;
using Backend.Enums;
using Backend.Utilities.AbstractFactory;

#endregion

namespace Backend.Utilities;

public class Game
{
    private static readonly Game _game = new ();
    private static List<Player> _connectedPlayers { get; set; }
    private static Subject _mapSubject { get; set; }
    private static EmptyMapFactory _emptyMapFactory = new ();
    private static PlusMapFactory _plusMapFactory = new ();
    private static OMapFactory _oMapFactory = new ();
    private static RandomMapFactory _randomMapFactory = new ();

    public static bool IsGameStarting =>
        _connectedPlayers.Count > 1 && _connectedPlayers.All(p => p.IsReady);

    private static int CurrentPlayerTurn = 0;

    private Game()
    {
        _connectedPlayers = new List<Player>(4);
        _mapSubject = new Subject();
    }

    public static Game GetGameInstance()
    {
        return _game;
    }
    
    public string NextPlayer()
    {
        var connectionId = _connectedPlayers[CurrentPlayerTurn].ConnectionID;
        CurrentPlayerTurn = CurrentPlayerTurn == _connectedPlayers.Count - 1 ? 0 : CurrentPlayerTurn + 1;
        return connectionId;
    }

    public AddPlayerState AddPlayer(Player player)
    {
        if (_connectedPlayers.Any(p => p.Name == player.Name)) return AddPlayerState.PlayerWithNameExists;

        if (_connectedPlayers.Count == 4) return AddPlayerState.ServerIsFull;

        if (IsGameStarting) return AddPlayerState.GameInProgress;

        _connectedPlayers.Add(player);
        _mapSubject.Subscribe(player);
        return AddPlayerState.Completed;
    }

    public void RemovePlayer(Player player)
    {
        _connectedPlayers.Remove(player);
        _mapSubject.Unsubscribe(player);
    }

    public Player? GetPlayerByConnectionId(string connectionId)
    {
        return _connectedPlayers.FirstOrDefault(p => p.ConnectionID == connectionId);
    }

    public List<Player> GetPlayers()
    {
        return _connectedPlayers.ToList();
    }

    public bool ChangeReadyStatus(string connectionId)
    {
        var player = _connectedPlayers.FirstOrDefault(p => p.ConnectionID == connectionId);
        if (player is not null) player.IsReady = !player.IsReady;

        return player?.IsReady ?? false;
    }

    public Map GenerateMap()
    {
        _mapSubject.Map = new PlusMapFactory().GenerateMap(_connectedPlayers);
        return _mapSubject.Map;
    }

    public void ChangeMap(MapType type)
    {
        var newMap = type switch
        {
            MapType.Empty => _emptyMapFactory.GenerateMap(_connectedPlayers),
            MapType.Plus => _plusMapFactory.GenerateMap(_connectedPlayers),
            MapType.O => _oMapFactory.GenerateMap(_connectedPlayers),
            MapType.Random => _randomMapFactory.GenerateMap(_connectedPlayers),
        };
        _mapSubject.Map = newMap;
    }
    
    public void MoveItem(int oldX, int oldY, int newX, int newY)
    {
        var newMap = _mapSubject.Map;
        (newMap.Tiles[oldX, oldY], newMap.Tiles[newX, newY]) = (newMap.Tiles[newX, newY], newMap.Tiles[oldX, oldY]);
        _mapSubject.Map = newMap;
    }

    public void RefreshMoves()
    {
        foreach (var player in _connectedPlayers)
        {
            foreach (var unit in player.Units)
            {
                unit.RemainingTurns = unit.MovesPerTurn;
            }
        }
    }

    public Map GetMap()
    {
        return _mapSubject.Map;
    }

    public Color GetFirstAvailableFreeColor()
    {
        foreach (var color in Enum.GetValues<Color>())
        {
            if (_connectedPlayers.All(p => p.Color != color))
            {
                return color;
            }
        }

        return Color.Yellow;
    }
}