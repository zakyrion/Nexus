using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public static class MessagePool
{
	static ConcurrentBag<Message> _messages = new ConcurrentBag<Message>();

	public static Message GetMessage()
	{
		if (_messages.IsEmpty || _messages.TryTake(out var result))
			result = new Message();
	
		return result;
	}

	public static void PutMessage(Message msg)
	{
		_messages.Add(msg);
	}
}
