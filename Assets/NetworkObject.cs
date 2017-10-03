using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

[Serializable]
public class NetworkObject {
	public byte networkId { get; protected set; }
	public NetworkObject(byte networkId) {
		this.networkId = networkId;
	}
}

[Serializable]
public class TextMessage : NetworkObject {
		public string message;
		public TextMessage(string message) : base(0) {
			this.message = message;
		}
	}
