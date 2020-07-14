using System;
using Nexus.Core;

public class Message : ICloneable
{
	public void Init(Address to, Address @from, object data, DeliveryType delivery = DeliveryType.ToOne,
		IResultContainer resultContainer = null)
	{
		From = @from;
		To = to;
		Data = data;
		Delivery = delivery;
		ResultContainer = resultContainer;
	}

	public object Data { get; private set; }
	public Address To { get; private set; }
	public Address From { get; private set; }
	public DeliveryType Delivery { get; private set; }
	public IResultContainer ResultContainer { get; private set; }

	public bool IsRequest => ResultContainer != null;

	public new Type GetType()
	{
		return Data.GetType();
	}

	public enum DeliveryType
	{
		ToOne = 0,
		ToAllInNode,
		ToChildren = 2,
		
		ChannelMessages = 100,
		
		
		ActorMessages = 200,
		
		
		NodeMessages = 300,
		GetChannel = 301
	}

	public object Clone()
	{
		throw new NotImplementedException();
	}
}