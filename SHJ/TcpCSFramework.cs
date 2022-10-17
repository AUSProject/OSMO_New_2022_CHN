/// <summary> 
/// 
/// �ļ�����: TcpCSFramework.cs 
/// �ļ�ID: 
/// �������: C# 
/// �ļ�˵��: �ṩTCP��������C/S��ͨѶ���ܻ����� 
/// (ʹ���첽Socket���ʵ��) 
/// 
/// ��ǰ�汾: 1.1 
/// �滻�汾: 1.0 
/// 
/// ����: ����� 
/// EMail: dyj057@gmail.com 
/// ��������: 2005-3-9 
/// ����޸�����: 2005-3-17 
/// 
/// ��ʷ�޸ļ�¼: 
/// 
/// ʱ��: 2005-3-14 
/// �޸�����: 
/// 1.����Ibms.Net.TcpCSFramework�����ռ�����Session����. 
/// 2.�޸�NetEventArgs��,����Ӧ����Ӷ���. 
/// 3.����˻Ự�˳�����,���ʺ�ʵ�ʵ����. 
/// ע��: 
/// * ǿ���˳�������Ӧ�ó���ֱ�ӽ���,����ͨ��������������� 
/// ������߳����쳣�˳���,û��ִ���������˳�������������. 
/// * �������˳�������Ӧ�ó���ִ���������˳��ķ����ؼ����� 
/// ��Ҫ����Socket.Shutdown( SocketShutdown.Both )��ŵ��� 
/// Socket.Close()����,������ֱ�ӵĵ���Socket.Close()����, 
/// ����������ý�����ǿ���˳�����. 
/// 
/// ʱ��: 2005-3-16 
/// �޸�����: 
/// 1.����TcpCli,Coder,DatagramResover����,�ѳ����ʵ�ֲ��ַ��� 
/// 2.�ļ��汾�޸�Ϊ1.1,1.0�汾��Ȼ����,����Ϊ: 
/// TcpCSFramework_v1.0.cs 
/// 3.��TcpServer���޸��Զ����hashtableΪϵͳHashtable���� 
/// 
/// </summary> 

using System; 
using System.Net.Sockets; 
using System.Net; 
using System.Text; 
using System.Diagnostics; 
using System.Collections; 

namespace Ibms.Net.TcpCSFramework 
{ 

	/// <summary> 
	/// ����ͨѶ�¼�ģ��ί�� 
	/// </summary> 
	public delegate void NetEvent(object sender, NetEventArgs e); 

	/// <summary> 
	/// �ṩTCP���ӷ���ķ������� 
	/// 
	/// �汾: 1.1 
	/// �滻�汾: 1.0 
	/// 
	/// �ص�: 
	/// 1.ʹ��hash�������������ӿͻ��˵�״̬���յ�����ʱ��ʵ�ֿ��ٲ���.ÿ�� 
	/// ��һ���µĿͻ������Ӿͻ����һ���µĻỰ(Session).��Session�����˿� 
	/// ���˶���. 
	/// 2.ʹ���첽��Socket�¼���Ϊ�������������ͨѶ����. 
	/// 3.֧�ִ���ǵ����ݱ��ĸ�ʽ��ʶ��,����ɴ����ݱ��ĵĴ������Ӧ���ӵ��� 
	/// �绷��.�����涨����֧�ֵ�������ݱ���Ϊ640K(��һ�����ݰ��Ĵ�С���ܴ��� 
	/// 640K,���������������Զ�ɾ����������,��Ϊ�ǷǷ�����),��ֹ��Ϊ���ݱ��� 
	/// �����Ƶ����������Ƿ��������� 
	/// 4.ͨѶ��ʽĬ��ʹ��Encoding.Default��ʽ�����Ϳ��Ժ���ǰ32λ����Ŀͻ��� 
	/// ͨѶ.Ҳ����ʹ��U-16��U-8�ĵ�ͨѶ��ʽ����.�����ڸ�DatagramResolver��� 
	/// �̳��������ر���ͽ��뺯��,�Զ�����ܸ�ʽ����ͨѶ.��֮ȷ���ͻ�������� 
	/// ����ʹ����ͬ��ͨѶ��ʽ 
	/// 5.ʹ��C# native code,��������Ч�ʵĿ��ǿ��Խ�C++����д�ɵ�32λdll������ 
	/// C#���Ĵ���, ��������ȱ������ֲ��,������Unsafe����(�����C++����Ҳ����) 
	/// 6.�������Ʒ�����������½�ͻ�����Ŀ 
	/// 7.��ʹ��TcpListener�ṩ���Ӿ�ϸ�Ŀ��ƺ͸���ǿ���첽���ݴ���Ĺ���,����Ϊ 
	/// TcpListener������� 
	/// 8.ʹ���첽ͨѶģʽ,��ȫ���õ���ͨѶ�������߳�����,���뿼��ͨѶ��ϸ�� 
	/// 
	/// ע��: 
	/// 1.���ֵĴ�����Rational XDE����,���������淶���� 
	/// 
	/// ԭ��: 
	/// 
	/// 
	/// ʹ���÷�: 
	/// 
	/// ����: 
	/// 
	/// </summary> 
	public class TcpSvr 
	{ 
		#region �����ֶ� 

		/// <summary> 
		/// Ĭ�ϵķ�����������ӿͻ��˶����� 
		/// </summary> 
		public const int DefaultMaxClient=100; 

		/// <summary> 
		/// �������ݻ�������С64K 
		/// </summary> 
		public const int DefaultBufferSize = 64*1024; 

		/// <summary> 
		/// ������ݱ��Ĵ�С 
		/// </summary> 
		public const int MaxDatagramSize = 640*1024; 

		/// <summary> 
		/// ���Ľ����� 
		/// </summary> 
		private DatagramResolver _resolver; 

		/// <summary> 
		/// ͨѶ��ʽ��������� 
		/// </summary> 
		private Coder _coder; 

		/// <summary> 
		/// ����������ʹ�õĶ˿� 
		/// </summary> 
		private ushort _port; 

		/// <summary> 
		/// ������������������ͻ��������� 
		/// </summary> 
		private ushort _maxClient; 

		/// <summary> 
		/// ������������״̬ 
		/// </summary> 
		private bool _isRun; 

		/// <summary> 
		/// �������ݻ����� 
		/// </summary> 
		private byte[] _recvDataBuffer; 

		/// <summary> 
		/// ������ʹ�õ��첽Socket��, 
		/// </summary> 
		private Socket _svrSock; 

		/// <summary> 
		/// �������пͻ��˻Ự�Ĺ�ϣ�� 
		/// </summary> 
		private Hashtable _sessionTable; 

		/// <summary> 
		/// ��ǰ�����ӵĿͻ����� 
		/// </summary> 
		private ushort _clientCount; 

		#endregion 

		#region �¼����� 

		/// <summary> 
		/// �ͻ��˽��������¼� 
		/// </summary> 
		public event NetEvent ClientConn; 

		/// <summary> 
		/// �ͻ��˹ر��¼� 
		/// </summary> 
		public event NetEvent ClientClose; 

		/// <summary> 
		/// �������Ѿ����¼� 
		/// </summary> 
		public event NetEvent ServerFull; 

		/// <summary> 
		/// ���������յ������¼� 
		/// </summary> 
		public event NetEvent RecvData; 

		#endregion 

		#region ���캯�� 

		/// <summary> 
		/// ���캯�� 
		/// </summary> 
		/// <param name="port">�������˼����Ķ˿ں�</param> 
		/// <param name="maxClient">�����������ɿͻ��˵��������</param> 
		/// <param name="encodingMothord">ͨѶ�ı��뷽ʽ</param> 
		public TcpSvr( ushort port,ushort maxClient, Coder coder) 
		{ 
			_port = port; 
			_maxClient = maxClient; 
			_coder = coder; 
		} 


		/// <summary> 
		/// ���캯��(Ĭ��ʹ��Default���뷽ʽ) 
		/// </summary> 
		/// <param name="port">�������˼����Ķ˿ں�</param> 
		/// <param name="maxClient">�����������ɿͻ��˵��������</param> 
		public TcpSvr( ushort port,ushort maxClient) 
		{ 
			_port = port; 
			_maxClient = maxClient; 
			_coder = new Coder(Coder.EncodingMothord.Default); 
		} 


		// <summary> 
		/// ���캯��(Ĭ��ʹ��Default���뷽ʽ��DefaultMaxClient(100)���ͻ��˵�����) 
		/// </summary> 
		/// <param name="port">�������˼����Ķ˿ں�</param> 
		public TcpSvr( ushort port):this( port, DefaultMaxClient) 
		{ 
		} 

		#endregion 

		#region ���� 

		/// <summary> 
		/// ��������Socket���� 
		/// </summary> 
		public Socket ServerSocket 
		{ 
			get 
			{ 
				return _svrSock; 
			} 
		} 

		/// <summary> 
		/// ���ݱ��ķ����� 
		/// </summary> 
		public DatagramResolver Resovlver 
		{ 
			get 
			{ 
				return _resolver; 
			} 
			set 
			{ 
				_resolver = value; 
			} 
		} 

		/// <summary> 
		/// �ͻ��˻Ự����,�������еĿͻ���,������Ը���������ݽ����޸� 
		/// </summary> 
		public Hashtable SessionTable 
		{ 
			get 
			{ 
				return _sessionTable; 
			} 
		} 

		/// <summary> 
		/// �������������ɿͻ��˵�������� 
		/// </summary> 
		public int Capacity 
		{ 
			get 
			{ 
				return _maxClient; 
			} 
		} 

		/// <summary> 
		/// ��ǰ�Ŀͻ��������� 
		/// </summary> 
		public int SessionCount 
		{ 
			get 
			{ 
				return _clientCount; 
			} 
		} 

		/// <summary> 
		/// ����������״̬ 
		/// </summary> 
		public bool IsRun 
		{ 
			get 
			{ 
				return _isRun; 
			} 

		} 

		#endregion 

		#region ���з��� 

		/// <summary> 
		/// ��������������,��ʼ�����ͻ������� 
		/// </summary> 
		public virtual void Start() 
		{ 
			if( _isRun ) 
			{ 
				throw (new ApplicationException("TcpSvr�Ѿ�������.")); 
			} 

			_sessionTable = new Hashtable(53); 

			_recvDataBuffer = new byte[DefaultBufferSize]; 

			//��ʼ��socket 
			_svrSock = new Socket( AddressFamily.InterNetwork, 
				SocketType.Stream, ProtocolType.Tcp ); 

			//�󶨶˿� 
			IPEndPoint iep = new IPEndPoint( IPAddress.Any, _port); 
			_svrSock.Bind(iep); 

			//��ʼ���� 
			_svrSock.Listen(5); 

			//�����첽�������ܿͻ������� 
			_svrSock.BeginAccept(new AsyncCallback( AcceptConn ), _svrSock); 

			_isRun = true; 

		} 

		/// <summary> 
		/// ֹͣ����������,������ͻ��˵����ӽ��ر� 
		/// </summary> 
		public virtual void Stop() 
		{ 
			if( !_isRun ) 
			{ 
				throw (new ApplicationException("TcpSvr�Ѿ�ֹͣ")); 
			} 

			//���������䣬һ��Ҫ�ڹر����пͻ�����ǰ���� 
			//������EndConn����ִ��� 
			_isRun = false; 

			//�ر���������,����ͻ��˻���Ϊ��ǿ�ƹر����� 
			if( _svrSock.Connected ) 
			{ 
				_svrSock.Shutdown( SocketShutdown.Both ); 
			} 

			CloseAllClient(); 

			//������Դ 
			_svrSock.Close(); 

			_sessionTable = null; 

		} 


		/// <summary> 
		/// �ر����еĿͻ��˻Ự,�����еĿͻ������ӻ�Ͽ� 
		/// </summary> 
		public virtual void CloseAllClient() 
		{ 
			foreach(Session client in _sessionTable.Values) 
			{ 
				client.Close(); 
			} 

			_sessionTable.Clear(); 
		} 


		/// <summary> 
		/// �ر�һ����ͻ���֮��ĻỰ 
		/// </summary> 
		/// <param name="closeClient">��Ҫ�رյĿͻ��˻Ự����</param> 
		public virtual void CloseSession(Session closeClient) 
		{ 
			Debug.Assert( closeClient !=null); 

			if( closeClient !=null ) 
			{ 

				closeClient.Datagram =null; 

				_sessionTable.Remove(closeClient.ID); 

				_clientCount--; 

				//�ͻ���ǿ�ƹر����� 
				if( ClientClose != null ) 
				{ 
					ClientClose(this, new NetEventArgs( closeClient )); 
				} 

				closeClient.Close(); 
			} 
		} 


		/// <summary> 
		/// �������� 
		/// </summary> 
		/// <param name="recvDataClient">�������ݵĿͻ��˻Ự</param> 
		/// <param name="datagram">���ݱ���</param> 
		public virtual void Send( Session recvDataClient, string datagram ) 
		{ 
			//������ݱ��� 
			byte [] data = _coder.GetEncodingBytes(datagram); 

			recvDataClient.ClientSocket.BeginSend( data, 0, data.Length, SocketFlags.None, 
				new AsyncCallback( SendDataEnd ), recvDataClient.ClientSocket ); 

		} 

		#endregion 

		#region �ܱ������� 
		/// <summary> 
		/// �ر�һ���ͻ���Socket,������Ҫ�ر�Session 
		/// </summary> 
		/// <param name="client">Ŀ��Socket����</param> 
		/// <param name="exitType">�ͻ����˳�������</param> 
		protected virtual void CloseClient( Socket client, Session.ExitType exitType) 
		{ 
			Debug.Assert ( client !=null); 

			//���Ҹÿͻ����Ƿ����,���������,�׳��쳣 
			Session closeClient = FindSession(client); 

			closeClient.TypeOfExit = exitType; 

			if(closeClient!=null) 
			{ 
				CloseSession(closeClient); 
			} 
			else 
			{ 
				throw( new ApplicationException("��Ҫ�رյ�Socket���󲻴���")); 
			} 
		} 


		/// <summary> 
		/// �ͻ������Ӵ����� 
		/// </summary> 
		/// <param name="iar">���������������ӵ�Socket����</param> 
		protected virtual void AcceptConn(IAsyncResult iar) 
		{ 
			//���������ֹͣ�˷���,�Ͳ����ٽ����µĿͻ��� 
			if( !_isRun) 
			{ 
				return; 
			} 

			//����һ���ͻ��˵��������� 
			Socket oldserver = ( Socket ) iar.AsyncState; 

			Socket client = oldserver.EndAccept(iar); 

			//����Ƿ�ﵽ��������Ŀͻ�����Ŀ 
			if( _clientCount == _maxClient ) 
			{ 
				//����������,����֪ͨ 
				if( ServerFull != null ) 
				{ 
					ServerFull(this, new NetEventArgs( new Session(client))); 
				} 

			} 
			else 
			{ 

				Session newSession = new Session( client ); 

				_sessionTable.Add(newSession.ID, newSession); 

				//�ͻ������ü���+1 
				_clientCount ++; 

				//��ʼ�������Ըÿͻ��˵����� 
				client.BeginReceive( _recvDataBuffer,0 , _recvDataBuffer.Length, SocketFlags.None, 
					new AsyncCallback(ReceiveData), client); 

				//�µĿͻ�������,����֪ͨ 
				if( ClientConn != null ) 
				{ 
					ClientConn(this, new NetEventArgs(newSession ) ); 
				} 
			} 

			//�������ܿͻ��� 
			_svrSock.BeginAccept(new AsyncCallback( AcceptConn ), _svrSock); 
		} 


		/// <summary> 
		/// ͨ��Socket�������Session���� 
		/// </summary> 
		/// <param name="client"></param> 
		/// <returns>�ҵ���Session����,���Ϊnull,˵���������ڸûػ�</returns> 
		private Session FindSession( Socket client ) 
		{ 
			SessionId id = new SessionId((int)client.Handle); 

			return (Session)_sessionTable[id]; 
		} 


		/// <summary> 
		/// ����������ɴ��������첽�����Ծ���������������У� 
		/// �յ����ݺ󣬻��Զ�����Ϊ�ַ������� 
		/// </summary> 
		/// <param name="iar">Ŀ��ͻ���Socket</param> 
		protected virtual void ReceiveData(IAsyncResult iar) 
		{ 
			Socket client = (Socket)iar.AsyncState; 

			try 
			{ 
				//������ο�ʼ���첽�Ľ���,���Ե��ͻ����˳���ʱ�� 
				//������ִ��EndReceive 

				int recv = client.EndReceive(iar); 

				if( recv == 0 ) 
				{ 
					//�����Ĺر� 
					CloseClient(client, Session.ExitType.NormalExit); 
					return; 
				} 

				string receivedData = _coder.GetEncodingString( _recvDataBuffer, recv ); 

				//�����յ����ݵ��¼� 
				if(RecvData!=null) 
				{ 
					Session sendDataSession= FindSession(client); 

					Debug.Assert( sendDataSession!=null ); 

					//��������˱��ĵ�β���,��Ҫ�����ĵĶ������ 
					if(_resolver != null) 
					{ 
						if( sendDataSession.Datagram !=null && 
							sendDataSession.Datagram.Length !=0) 
						{ 
							//�������һ��ͨѶʣ��ı���Ƭ�� 
							receivedData= sendDataSession.Datagram + receivedData ; 
						} 

						string [] recvDatagrams = _resolver.Resolve(ref receivedData); 


						foreach(string newDatagram in recvDatagrams) 
						{ 
							//���,Ϊ�˱���Datagram�Ķ����� 
							ICloneable copySession = (ICloneable)sendDataSession; 

							Session clientSession = (Session)copySession.Clone(); 

							clientSession.Datagram = newDatagram; 
							//����һ��������Ϣ 
							RecvData(this,new NetEventArgs( clientSession )); 
						} 

						//ʣ��Ĵ���Ƭ��,�´ν��յ�ʱ��ʹ�� 
						sendDataSession.Datagram = receivedData; 

						if( sendDataSession.Datagram.Length > MaxDatagramSize ) 
						{ 
							sendDataSession.Datagram = null; 
						} 

					} 
						//û�ж��屨�ĵ�β���,ֱ�ӽ�����Ϣ������ʹ�� 
					else 
					{ 
						ICloneable copySession = (ICloneable)sendDataSession; 

						Session clientSession = (Session)copySession.Clone(); 

						clientSession.Datagram = receivedData; 

						RecvData(this,new NetEventArgs( clientSession )); 
					} 

				}//end of if(RecvData!=null) 

				//���������������ͻ��˵����� 
				client.BeginReceive( _recvDataBuffer, 0, _recvDataBuffer.Length , SocketFlags.None, 
					new AsyncCallback( ReceiveData ), client); 

			} 
			catch(SocketException ex) 
			{ 
				//�ͻ����˳� 
				if( 10054 == ex.ErrorCode ) 
				{ 
					//�ͻ���ǿ�ƹر� 
					CloseClient(client, Session.ExitType.ExceptionExit); 
				} 

			} 
			catch(ObjectDisposedException ex) 
			{ 
				//�����ʵ�ֲ������� 
				//������CloseSession()ʱ,��������ݽ���,�������ݽ��� 
				//�����л����int recv = client.EndReceive(iar); 
				//�ͷ�����CloseSession()�Ѿ����õĶ��� 
				//����������ʵ�ַ���Ҳ�����˴��ŵ�. 
				if(ex!=null) 
				{ 
					ex=null; 
					//DoNothing; 
				} 
			} 

		} 


		/// <summary> 
		/// ����������ɴ����� 
		/// </summary> 
		/// <param name="iar">Ŀ��ͻ���Socket</param> 
		protected virtual void SendDataEnd(IAsyncResult iar) 
		{ 
			Socket client = (Socket)iar.AsyncState; 

			int sent = client.EndSend(iar); 
		} 

		#endregion 

	} 


	/// <summary> 
	/// �ṩTcp�������ӷ���Ŀͻ����� 
	/// 
	/// �汾: 1.0 
	/// �滻�汾: 
	/// 
	/// ����: 
	/// ԭ��: 
	/// 1.ʹ���첽SocketͨѶ�����������һ����ͨѶ��ʽͨѶ,��ע�����������ͨ 
	/// Ѷ��ʽһ��Ҫһ��,���������ɷ������������,��������û�п˷�,��ô��byte[] 
	/// �ж����ı����ʽ 
	/// 2.֧�ִ���ǵ����ݱ��ĸ�ʽ��ʶ��,����ɴ����ݱ��ĵĴ������Ӧ���ӵ��� 
	/// �绷��. 
	/// �÷�: 
	/// ע��: 
	/// </summary> 
	public class TcpCli 
	{ 
		#region �ֶ� 

		/// <summary> 
		/// �ͻ����������֮��ĻỰ�� 
		/// </summary> 
		private Session _session; 

		/// <summary> 
		/// �ͻ����Ƿ��Ѿ����ӷ����� 
		/// </summary> 
		private bool _isConnected = false; 

		/// <summary> 
		/// �������ݻ�������С64*1024bits 
		/// </summary> 
		public const int DefaultBufferSize = 64*1024; 

		/// <summary> 
		/// ���Ľ����� 
		/// </summary> 
		private DatagramResolver _resolver; 

		/// <summary> 
		/// ͨѶ��ʽ��������� 
		/// </summary> 
		private Coder _coder; 

		/// <summary> 
		/// �������ݻ����� 
		/// </summary> 
		private byte[] _recvDataBuffer = new byte[DefaultBufferSize]; 

		#endregion 

		#region �¼����� 

		//��Ҫ�����¼������յ��¼���֪ͨ������������˳�������ȡ������ 

		/// <summary> 
		/// �Ѿ����ӷ������¼� 
		/// </summary> 
		public event NetEvent ConnectedServer; 

		/// <summary> 
		/// ���յ����ݱ����¼� 
		/// </summary> 
		public event NetEvent ReceivedDatagram; 

		/// <summary> 
		/// ���ӶϿ��¼� 
		/// </summary> 
		public event NetEvent DisConnectedServer; 
		#endregion 

		#region ���� 

		/// <summary> 
		/// ���ؿͻ����������֮��ĻỰ���� 
		/// </summary> 
		public Session ClientSession 
		{ 
			get 
			{ 
				return _session; 
			} 
		} 

		/// <summary> 
		/// ���ؿͻ����������֮�������״̬ 
		/// </summary> 
		public bool IsConnected 
		{ 
			get 
			{ 
				return _isConnected; 
			} 
		} 

		/// <summary> 
		/// ���ݱ��ķ����� 
		/// </summary> 
		public DatagramResolver Resovlver 
		{ 
			get 
			{ 
				return _resolver; 
			} 
			set 
			{ 
				_resolver = value; 
			} 
		} 

		/// <summary> 
		/// ��������� 
		/// </summary> 
		public Coder ServerCoder 
		{ 
			get 
			{ 
				return _coder; 
			} 
		} 

		#endregion 

		#region ���з��� 

		/// <summary> 
		/// Ĭ�Ϲ��캯��,ʹ��Ĭ�ϵı����ʽ 
		/// </summary> 
		public TcpCli() 
		{ 
			_coder = new Coder( Coder.EncodingMothord.Default ); 
		} 

		/// <summary> 
		/// ���캯��,ʹ��һ���ض��ı���������ʼ�� 
		/// </summary> 
		/// <param name="_coder">���ı�����</param> 
		public TcpCli( Coder coder ) 
		{ 
			_coder = coder; 
		} 

		/// <summary> 
		/// ���ӷ����� 
		/// </summary> 
		/// <param name="ip">������IP��ַ</param> 
		/// <param name="port">�������˿�</param> 
		public virtual void Connect( string ip, int port) 
		{ 
			if(IsConnected) 
			{ 
				//�������� 
				Debug.Assert( _session !=null); 

				Close(); 
			} 

			Socket newsock= new Socket(AddressFamily.InterNetwork, 
				SocketType.Stream, ProtocolType.Tcp); 

			IPEndPoint iep = new IPEndPoint( IPAddress.Parse(ip), port); 
			newsock.BeginConnect(iep, new AsyncCallback(Connected), newsock); 

		} 

		/// <summary> 
		/// �������ݱ��� 
		/// </summary> 
		/// <param name="datagram"></param> 
		public virtual void Send( string datagram) 
		{ 
			if(datagram.Length ==0 ) 
			{ 
				return; 
			} 

			if( _isConnected ) 
			{
                //throw (new ApplicationException("û�����ӷ����������ܷ�������") ); 


                //��ñ��ĵı����ֽ� 
                byte[] data = _coder.GetEncodingBytes(datagram);

                _session.ClientSocket.BeginSend(data, 0, data.Length, SocketFlags.None,
                    new AsyncCallback(SendDataEnd), _session.ClientSocket);
            }
		}

        /// <summary> 
        /// �������ݱ��� 
        /// </summary> 
        /// <param name="datagram"></param> 
        public virtual void Sendbytes(byte[] datagram,int bytescount)
        {
            try
            {
                if (bytescount == 0)
                {
                    return;
                }

                if (_isConnected)
                {
                    //throw (new ApplicationException("û�����ӷ����������ܷ�������"));


                    _session.ClientSocket.BeginSend(datagram, 0, bytescount, SocketFlags.None,
                        new AsyncCallback(SendDataEnd), _session.ClientSocket);
                }
            }
            catch
            {
            }
        }

		/// <summary> 
		/// �ر����� 
		/// </summary> 
		public virtual void Close() 
		{ 
			if(!_isConnected) 
			{ 
				return; 
			} 

			_session.Close(); 

			_session = null; 

			_isConnected = false; 
		} 

		#endregion 

		#region �ܱ������� 

		/// <summary> 
		/// ���ݷ�����ɴ����� 
		/// </summary> 
		/// <param name="iar"></param> 
		protected virtual void SendDataEnd(IAsyncResult iar) 
		{ 
			Socket remote = (Socket)iar.AsyncState; 
			int sent = remote.EndSend(iar); 
			Debug.Assert(sent !=0); 

		} 

		/// <summary> 
		/// ����Tcp���Ӻ������ 
		/// </summary> 
		/// <param name="iar">�첽Socket</param> 
		protected virtual void Connected(IAsyncResult iar) 
		{ 
			try
			{
				Socket socket = (Socket)iar.AsyncState; 

				socket.EndConnect(iar); 

				//�����µĻỰ 
				_session = new Session(socket); 

				//���ӳ�ʱ����							2005-6-6.
				//_session.ClientSocket.SetSocketOption(SocketOptionLevel.Socket,SocketOptionName.ReceiveTimeout,5000);

				_isConnected = true; 

				//�������ӽ����¼� 
				if(ConnectedServer != null) 
				{ 
					ConnectedServer(this, new NetEventArgs(_session)); 
				} 

				//�������Ӻ�Ӧ�������������� 
				_session.ClientSocket.BeginReceive(_recvDataBuffer, 0, 
					DefaultBufferSize, SocketFlags.None, 
					new AsyncCallback(RecvData), socket); 
			}
			catch(SocketException nete)
			{
				if(nete != null){}	//do nothing
			}
			catch(ObjectDisposedException ex) 
			{ 
				if(ex != null){}	//do nothing
			} 
			catch(Exception e)
			{
				if(e != null){}
			}
		} 

		/// <summary> 
		/// ���ݽ��մ����� 
		/// </summary> 
		/// <param name="iar">�첽Socket</param> 
		protected virtual void RecvData(IAsyncResult iar) 
		{ 
			Socket remote = (Socket)iar.AsyncState;

			try 
			{ 
				int recv = remote.EndReceive(iar); 

				//�������˳� 
				if(recv ==0 ) 
				{ 
					_session.TypeOfExit = Session.ExitType.NormalExit; 

					if(DisConnectedServer!=null) 
					{ 
						DisConnectedServer(this, new NetEventArgs(_session)); 
					} 

					return; 
				} 

				string receivedData = _coder.GetEncodingString( _recvDataBuffer,recv ); 

				//ͨ���¼������յ��ı��� 
				if(ReceivedDatagram != null) 
				{ 
					//ͨ�����Ľ��������������� 
					//��������˱��ĵ�β���,��Ҫ�����ĵĶ������ 
					if(_resolver != null) 
					{ 
						if( _session.Datagram !=null && 
							_session.Datagram.Length !=0) 
						{ 
							//�������һ��ͨѶʣ��ı���Ƭ�� 
							receivedData= _session.Datagram + receivedData ; 
						} 

						string [] recvDatagrams = _resolver.Resolve(ref receivedData); 


						foreach(string newDatagram in recvDatagrams) 
						{ 
							//Need Deep Copy.��Ϊ��Ҫ��֤�����ͬ���Ķ������� 
							ICloneable copySession = (ICloneable)_session; 

							Session clientSession = (Session)copySession.Clone(); 

							clientSession.Datagram = newDatagram; 

							//����һ��������Ϣ 
							ReceivedDatagram(this,new NetEventArgs( clientSession )); 
						} 

						//ʣ��Ĵ���Ƭ��,�´ν��յ�ʱ��ʹ�� 
						_session.Datagram = receivedData; 
					} 
						//û�ж��屨�ĵ�β���,ֱ�ӽ�����Ϣ������ʹ�� 
					else 
					{ 
						ICloneable copySession = (ICloneable)_session; 

						Session clientSession = (Session)copySession.Clone(); 

						clientSession.Datagram = receivedData; 

						ReceivedDatagram( this, new NetEventArgs( clientSession )); 

					} 


				}//end of if(ReceivedDatagram != null) 

				//������������ 
				_session.ClientSocket.BeginReceive(_recvDataBuffer, 0, DefaultBufferSize, SocketFlags.None, 
					new AsyncCallback(RecvData), _session.ClientSocket); 
			} 
			catch(SocketException ex) 
			{ 
				//�ͻ����˳� 
				if( 10054 == ex.ErrorCode ) 
				{ 
					//������ǿ�ƵĹر����ӣ�ǿ���˳� 
				}
				_session.TypeOfExit = Session.ExitType.ExceptionExit; 

				if(DisConnectedServer!=null) 
				{ 
					DisConnectedServer(this, new NetEventArgs(_session)); 
				} 
			} 
			catch(ObjectDisposedException ex) 
			{ 
				//�����ʵ�ֲ������� 
				//������CloseSession()ʱ,��������ݽ���,�������ݽ��� 
				//�����л����int recv = client.EndReceive(iar); 
				//�ͷ�����CloseSession()�Ѿ����õĶ��� 
				//����������ʵ�ַ���Ҳ�����˴��ŵ�. 
				if(ex!=null) 
				{ 
					ex =null; 
					//DoNothing; 
				} 
			}
			catch(Exception e)
			{
				if(e != null){}
			}

		} 

		#endregion 


	} 

	/// <summary> 
	/// ͨѶ�����ʽ�ṩ��,ΪͨѶ�����ṩ����ͽ������ 
	/// ������ڼ̳����ж����Լ��ı��뷽ʽ��:���ݼ��ܴ���� 
	/// </summary> 
	public class Coder 
	{ 
		/// <summary> 
		/// ���뷽ʽ 
		/// </summary> 
		private EncodingMothord _encodingMothord; 

		protected Coder() 
		{ 

		} 

		public Coder(EncodingMothord encodingMothord) 
		{ 
			_encodingMothord = encodingMothord; 
		} 

		public enum EncodingMothord 
		{ 
			Default =0, 
			Unicode, 
			UTF8, 
			ASCII, 
		} 

		/// <summary> 
		/// ͨѶ���ݽ��� 
		/// </summary> 
		/// <param name="dataBytes">��Ҫ���������</param> 
		/// <returns>����������</returns> 
		public virtual string GetEncodingString( byte [] dataBytes,int size) 
		{ 
			switch( _encodingMothord ) 
			{ 
				case EncodingMothord.Default: 
				{ 
					return Encoding.Default.GetString(dataBytes,0,size); 
				} 
				case EncodingMothord.Unicode: 
				{ 
					return Encoding.Unicode.GetString(dataBytes,0,size); 
				} 
				case EncodingMothord.UTF8: 
				{ 
					return Encoding.UTF8.GetString(dataBytes,0,size); 
				} 
				case EncodingMothord.ASCII: 
				{ 
					return Encoding.ASCII.GetString(dataBytes,0,size); 
				} 
				default: 
				{ 
					throw( new Exception("δ����ı����ʽ")); 
				} 
			} 

		} 

		/// <summary> 
		/// ���ݱ��� 
		/// </summary> 
		/// <param name="datagram">��Ҫ����ı���</param> 
		/// <returns>����������</returns> 
		public virtual byte[] GetEncodingBytes(string datagram) 
		{ 
			switch( _encodingMothord) 
			{ 
				case EncodingMothord.Default: 
				{ 
					return Encoding.Default.GetBytes(datagram); 
				} 
				case EncodingMothord.Unicode: 
				{ 
					return Encoding.Unicode.GetBytes(datagram); 
				} 
				case EncodingMothord.UTF8: 
				{ 
					return Encoding.UTF8.GetBytes(datagram); 
				} 
				case EncodingMothord.ASCII: 
				{ 
					return Encoding.ASCII.GetBytes(datagram); 
				} 
				default: 
				{ 
					throw( new Exception("δ����ı����ʽ")); 
				} 
			} 
		} 

	} 


	/// <summary> 
	/// ���ݱ��ķ�����,ͨ���������յ���ԭʼ����,�õ����������ݱ���. 
	/// �̳и������ʵ���Լ��ı��Ľ�������. 
	/// ͨ���ı���ʶ�𷽷�����:�̶�����,���ȱ��,��Ƿ��ȷ��� 
	/// �������ʵ���Ǳ�Ƿ��ķ���,������ڼ̳�����ʵ�������ķ��� 
	/// </summary> 
	public class DatagramResolver 
	{ 
		/// <summary> 
		/// ���Ľ������ 
		/// </summary> 
		private string endTag; 

		/// <summary> 
		/// ���ؽ������ 
		/// </summary> 
		string EndTag 
		{ 
			get 
			{ 
				return endTag; 
			} 
		} 

		/// <summary> 
		/// �ܱ�����Ĭ�Ϲ��캯��,�ṩ���̳���ʹ�� 
		/// </summary> 
		protected DatagramResolver() 
		{ 

		} 

		/// <summary> 
		/// ���캯�� 
		/// </summary> 
		/// <param name="endTag">���Ľ������</param> 
		public DatagramResolver(string endTag) 
		{ 
			if(endTag == null) 
			{ 
				throw (new ArgumentNullException("������ǲ���Ϊnull")); 
			} 

			if(endTag == "") 
			{ 
				throw (new ArgumentException("������Ƿ��Ų���Ϊ���ַ���")); 
			} 

			this.endTag = endTag; 
		} 

		/// <summary> 
		/// �������� 
		/// </summary> 
		/// <param name="rawDatagram">ԭʼ����,����δʹ�õı���Ƭ��, 
		/// ��Ƭ�ϻᱣ����Session��Datagram������</param> 
		/// <returns>��������,ԭʼ���ݿ��ܰ����������</returns> 
		public virtual string [] Resolve(ref string rawDatagram) 
		{ 
			ArrayList datagrams = new ArrayList(); 

			//ĩβ���λ������ 
			int tagIndex =-1; 

			while(true) 
			{ 
				tagIndex = rawDatagram.IndexOf(endTag,tagIndex+1); 

				if( tagIndex == -1 ) 
				{ 
					break; 
				} 
				else 
				{ 
					//����ĩβ��ǰ��ַ�����Ϊ������������ 
					string newDatagram = rawDatagram.Substring( 
						0, tagIndex/*+endTag.Length*/); 

					datagrams.Add(newDatagram); 

					if(tagIndex+endTag.Length >= rawDatagram.Length) 
					{ 
						rawDatagram=""; 

						break; 
					} 

					rawDatagram = rawDatagram.Substring(tagIndex+endTag.Length, 
						rawDatagram.Length - newDatagram.Length); 

					//�ӿ�ʼλ�ÿ�ʼ���� 
					tagIndex=0; 
				} 
			} 

			string [] results= new string[datagrams.Count]; 

			datagrams.CopyTo(results); 

			return results; 
		} 

	} 


	/// <summary> 
	/// �ͻ����������֮��ĻỰ�� 
	/// 
	/// �汾: 1.1 
	/// �滻�汾: 1.0 
	/// 
	/// ˵��: 
	/// �Ự�����Զ��ͨѶ�˵�״̬,��Щ״̬����Socket,��������, 
	/// �ͻ����˳�������(�����ر�,ǿ���˳���������) 
	/// </summary> 
	public class Session:ICloneable 
	{ 
		#region �ֶ� 

		/// <summary> 
		/// �ỰID 
		/// </summary> 
		private SessionId _id; 

		/// <summary> 
		/// �ͻ��˷��͵��������ı��� 
		/// ע��:����Щ����±��Ŀ���ֻ�Ǳ��ĵ�Ƭ�϶������� 
		/// </summary> 
		private string _datagram; 

		/// <summary> 
		/// �ͻ��˵�Socket 
		/// </summary> 
		private Socket _cliSock; 

		/// <summary> 
		/// �ͻ��˵��˳����� 
		/// </summary> 
		private ExitType _exitType; 

		/// <summary> 
		/// �˳�����ö�� 
		/// </summary> 
		public enum ExitType 
		{ 
			NormalExit , 
			ExceptionExit 
		}; 

		#endregion 

		#region ���� 

		/// <summary> 
		/// ���ػỰ��ID 
		/// </summary> 
		public SessionId ID 
		{ 
			get 
			{ 
				return _id; 
			} 
		} 

		/// <summary> 
		/// ��ȡ�Ự�ı��� 
		/// </summary> 
		public string Datagram 
		{ 
			get 
			{ 
				return _datagram; 
			} 
			set 
			{ 
				_datagram = value; 
			} 
		} 

		/// <summary> 
		/// �����ͻ��˻Ự������Socket���� 
		/// </summary> 
		public Socket ClientSocket 
		{ 
			get 
			{ 
				return _cliSock; 
			} 
		} 

		/// <summary> 
		/// ��ȡ�ͻ��˵��˳���ʽ 
		/// </summary> 
		public ExitType TypeOfExit 
		{ 
			get 
			{ 
				return _exitType; 
			} 

			set 
			{ 
				_exitType = value; 
			} 
		} 

		#endregion 

		#region ���� 

		/// <summary> 
		/// ʹ��Socket�����Handleֵ��ΪHashCode,���������õ���������. 
		/// </summary> 
		/// <returns></returns> 
		public override int GetHashCode() 
		{ 
			return (int)_cliSock.Handle; 
		} 

		/// <summary> 
		/// ��������Session�Ƿ����ͬһ���ͻ��� 
		/// </summary> 
		/// <param name="obj"></param> 
		/// <returns></returns> 
		public override bool Equals(object obj) 
		{ 
			Session rightObj = (Session)obj; 

			return (int)_cliSock.Handle == (int)rightObj.ClientSocket.Handle; 

		} 

		/// <summary> 
		/// ����ToString()����,����Session��������� 
		/// </summary> 
		/// <returns></returns> 
		public override string ToString() 
		{ 
			string result = string.Format("Session:{0},IP:{1}", 
				_id,_cliSock.RemoteEndPoint.ToString()); 

			//result.C 
			return result; 
		} 

		/// <summary> 
		/// ���캯�� 
		/// </summary> 
		/// <param name="cliSock">�Ựʹ�õ�Socket����</param> 
		public Session( Socket cliSock) 
		{ 
			Debug.Assert( cliSock !=null ); 

			_cliSock = cliSock; 

			_id = new SessionId( (int)cliSock.Handle); 
		} 

		/// <summary> 
		/// �رջỰ 
		/// </summary> 
		public void Close() 
		{ 
			Debug.Assert( _cliSock !=null ); 

			//�ر����ݵĽ��ܺͷ��� 
			_cliSock.Shutdown( SocketShutdown.Both ); 

			//������Դ 
			_cliSock.Close(); 
		} 

		#endregion 

		#region ICloneable ��Ա 

		object System.ICloneable.Clone() 
		{ 
			Session newSession = new Session(_cliSock); 
			newSession.Datagram = _datagram; 
			newSession.TypeOfExit = _exitType; 

			return newSession; 
		} 

		#endregion 
	} 


	/// <summary> 
	/// Ψһ�ı�־һ��Session,����Session������Hash��������ض����� 
	/// </summary> 
	public class SessionId 
	{ 
		/// <summary> 
		/// ��Session�����Socket�����Handleֵ��ͬ,���������ֵ����ʼ���� 
		/// </summary> 
		private int _id; 

		/// <summary> 
		/// ����IDֵ 
		/// </summary> 
		public int ID 
		{ 
			get 
			{ 
				return _id; 
			} 
		} 

		/// <summary> 
		/// ���캯�� 
		/// </summary> 
		/// <param name="id">Socket��Handleֵ</param> 
		public SessionId(int id) 
		{ 
			_id = id; 
		} 

		/// <summary> 
		/// ����.Ϊ�˷���Hashtable��ֵ���� 
		/// </summary> 
		/// <param name="obj"></param> 
		/// <returns></returns> 
		public override bool Equals(object obj) 
		{ 
			if(obj != null ) 
			{ 
				SessionId right = (SessionId) obj; 

				return _id == right._id; 
			} 
			else if(this == null) 
			{ 
				return true; 
			} 
			else 
			{ 
				return false; 
			} 

		} 

		/// <summary> 
		/// ����.Ϊ�˷���Hashtable��ֵ���� 
		/// </summary> 
		/// <returns></returns> 
		public override int GetHashCode() 
		{ 
			return _id; 
		} 

		/// <summary> 
		/// ����,Ϊ�˷�����ʾ��� 
		/// </summary> 
		/// <returns></returns> 
		public override string ToString() 
		{ 
			return _id.ToString (); 
		} 

	} 


	/// <summary> 
	/// ������������¼�����,�����˼������¼��ĻỰ���� 
	/// </summary> 
	public class NetEventArgs:EventArgs 
	{ 

		#region �ֶ� 

		/// <summary> 
		/// �ͻ����������֮��ĻỰ 
		/// </summary> 
		private Session _client; 

		#endregion 

		#region ���캯�� 
		/// <summary> 
		/// ���캯�� 
		/// </summary> 
		/// <param name="client">�ͻ��˻Ự</param> 
		public NetEventArgs(Session client) 
		{ 
			if( null == client) 
			{ 
				throw(new ArgumentNullException()); 
			} 

			_client = client; 
		} 
		#endregion 

		#region ���� 

		/// <summary> 
		/// ��ü������¼��ĻỰ���� 
		/// </summary> 
		public Session Client 
		{ 
			get 
			{ 
				return _client; 
			} 

		} 

		#endregion 

	} 
} 
