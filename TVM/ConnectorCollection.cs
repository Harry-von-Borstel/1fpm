// Copyright © 2012 by blueshell Software Engineering Harry von Borstel (http://www.blueshell.com)
// This work is licensed under COPL (see http://www.codeproject.com/info/cpol10.aspx)
// 
// 
using System.Collections.Generic;
using System.Linq;

namespace TypesaveViewModel
{
	public class ConnectorCollection
	{
		private readonly IEnumerable<IConnector> connectors;
		private readonly CollectionErrorInfo errorInfo;
		//public ConnectorCollection(IEnumerable<IConnector> connectors)
		public ConnectorCollection(params IConnector[] connectors)
		{
			this.connectors = connectors;
			foreach (var connector in connectors)
			{
				connector.ConnectorCollection = this;
			}
			this.errorInfo = new CollectionErrorInfo(this);
		}

		public ErrorInfo ErrorInfo
		{
			get { return this.errorInfo; }
		}

		public IEnumerable<IConnector> Connectors
		{
			get { return this.connectors; }
		}

		private class CollectionErrorInfo : ErrorInfo
		{
			private readonly ConnectorCollection collection;
			public CollectionErrorInfo(ConnectorCollection collection)
			{
				this.collection = collection;
			}

			public override bool HasError
			{
				get
				{
					return this.collection.Connectors.Any(connector => connector.ErrorInfo.HasError);
				}
			}

			public override IEnumerable<ErrorInfo> InnerErrors
			{
				get { return this.collection.Connectors.Select(connector => connector.ErrorInfo).Where(errorInfo => errorInfo.HasError); }
			}

			public override string ToString()
			{
				return this.InnerErrors.Aggregate("", (accu, errorInfo) => string.Format("{0}{2}{1}", accu, errorInfo, accu.Length > 0 ? "\n\n" : ""));
			}

		}
	}
}
