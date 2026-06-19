// Copyright (C) 2026 Dovintc
// This file is part of Syncra RPC
// Licensed under AGPL-3.0 with No-Misattribution Addendum.
// See LICENSE file for details.

using System.Runtime.InteropServices;
using System.IO;
using System.IO.Pipes;
using System.Text.Json;
using System;
using System.Collections.Generic;

namespace SyncraRPC;

public class PlatformNotSupportedException : Exception
{
    public PlatformNotSupportedException(string message) :base(message){}
}

public class Princess : IDisposable 
{
    private Random _random = new Random();
    public string ?CientId {get; private set;}
    public NamedPipeClientStream ?_pipe {get; set;}
    public int ?_nonce {get; set;}

    public Princess(string clientId){
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
            throw new PlatformNotSupportedException($"{RuntimeInformation.OSDescription} not supported, sorry(((");
        }
        this.CientId = clientId;
    }

    public void connect(){
        for (byte _ = 0; _ <= 11; _++){
            try{
                _pipe = new NamedPipeClientStream(".", $"discord-ipc-{_}", PipeDirection.InOut);
                _pipe.Connect(250);
                break;
            } catch {
                _pipe?.Dispose();
                _pipe = null;
            }
        }

        if (_pipe == null){
            throw new FileNotFoundException("Discord is not running. Please run the Discord client on your PC.");
        }

        Dictionary<string, object> handshake_data = new() {{"v", 1}, {"client_id", this.CientId?.ToString() ?? ""}};
        this.send(handshake_data, 0);
        this.read_pipe();
    }

    public void update(
        string? name = null,
        string? state = null,
        double? start = null,
        string? large_image = null,
        string? large_text = null,
        string? small_image = null,
        string? small_text = null
    ){
        this._nonce = _random.Next(1, 1001);
        
        var activity = new Dictionary<string, object>{
            {"name", name ?? ""},
        };

        if (state != null) activity["state"] = state;

        if (start.HasValue) {
            activity["timestamps"] = new Dictionary<string, object>{
                {"start", (long)(start.Value * 1000)}
            };
        }

        var assets = new Dictionary<string, object>();
        if (!string.IsNullOrWhiteSpace(large_image)) assets["large_image"] = large_image;
        if (!string.IsNullOrWhiteSpace(large_text)) assets["large_text"] = large_text;
        if (!string.IsNullOrWhiteSpace(small_image)) assets["small_image"] = small_image;
        if (!string.IsNullOrWhiteSpace(small_text)) assets["small_text"] = small_text;
        if (assets.Count > 0) activity["assets"] = assets;

        var args = new Dictionary<string, object>{
            {"pid", Environment.ProcessId},
            {"activity", activity}
        };

        Dictionary<string, object> data = new() {
            {"cmd", "SET_ACTIVITY"}, 
            {"args", args},
            {"nonce", _nonce.ToString() ?? "2676"}
        };

        this.send(data, 1);
    }

    private void send(Dictionary<string, object> data, uint opcode){
        byte[] json_byte = JsonSerializer.SerializeToUtf8Bytes(data);
        byte[] head = new byte[8];

        BitConverter.GetBytes(opcode).CopyTo(head, 0);
        BitConverter.GetBytes((uint)json_byte.Length).CopyTo(head, 4);

        if (_pipe != null && _pipe.IsConnected && _pipe.CanWrite){
            _pipe.Write(head, 0, head.Length);
            _pipe.Write(json_byte, 0, json_byte.Length);
            _pipe.Flush();
        } else {
            throw new IOException("Cant write data to pipe");
        }
    }

    public Dictionary<string, object> read_pipe(){
        byte[] buffer = new byte[8];
        if (_pipe != null && _pipe.IsConnected){
            int BytesRead = _pipe.Read(buffer, 0, buffer.Length);
            if (BytesRead == 8){
                uint opcode = BitConverter.ToUInt32(buffer, 0);
                uint length = BitConverter.ToUInt32(buffer, 4);
                
                byte[] JsonBuffer = new byte[length];

                int TotalBytesRead = 0;
                while(TotalBytesRead < length){
                    int read = _pipe.Read(JsonBuffer, TotalBytesRead, (int)length - TotalBytesRead);
                    if (read == 0) throw new IOException("The connection is terminated before the JSON reading is completed.");
                    TotalBytesRead += read;
                }

                var json_data = JsonSerializer.Deserialize<Dictionary<string, object>>(JsonBuffer)!;
                return json_data ?? new Dictionary<string, object>();
            } else {
                return new Dictionary<string, object>();
            }
        } else {
            throw new InvalidOperationException("Pipe is not connect or null");
        }
    }

    public void close(){
        if (this._pipe != null){
            this._pipe.Close();
            this._pipe.Dispose();
            this._pipe = null;
        }
    }

    public void Dispose(){
        this.close();
        GC.SuppressFinalize(this);
    }
}