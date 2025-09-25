Imports System.Net
Imports System.Net.Sockets
Imports System.Text

Public Class clsClient

#Region "Variable"
    Private m_client As TcpClient
    Private m_stream As NetworkStream
#End Region

#Region "Property"
    Public ReadOnly Property IpAddress As IPAddress
    Public ReadOnly Property Port As Integer

    Public Property Delimiter As String = vbCrLf

    Public ReadOnly Property ErrorMessage As String
#End Region

    Public Sub New()
        _IpAddress = New IPAddress({0, 0, 0, 0})
        _Port = 0
    End Sub


    Public Function Open(ByVal ipAddress As IPAddress, ByVal port As Integer) As Boolean

        m_client = New TcpClient()

        Try
            m_client.Connect(ipAddress, port)
            m_stream = m_client.GetStream

            m_stream.ReadTimeout = 5000

            _IpAddress = ipAddress
            _Port = port

        Catch ex As Exception
            _ErrorMessage = ex.Message

            Close()
            Return False
        End Try

        Return True
    End Function

    Public Function Close() As Boolean
        If m_client Is Nothing Then
            Return True
        End If

        Try
            If m_client.Connected = True Then
                m_stream.Close()
                m_client.Close()
            End If

        Catch ex As Exception
            _ErrorMessage = ex.Message
            Return False

        Finally
            '成否問わずDispose実行
            Try
                If m_stream IsNot Nothing Then
                    m_stream.Dispose()
                End If
            Catch exStream As Exception
                _ErrorMessage &= vbCrLf & exStream.Message

            End Try

            Try
                m_client.Dispose()

            Catch exClient As Exception
                _ErrorMessage &= vbCrLf & exClient.Message
            End Try
            m_stream = Nothing
            m_client = Nothing

            _IpAddress = New IPAddress({0, 0, 0, 0})
            _Port = 0
        End Try

        Return True
    End Function

    Public Function Send(ByVal message As String) As Boolean
        Try
            message &= Delimiter
            Dim data As Byte() = Encoding.UTF8.GetBytes(message)

            m_stream.Write(data, 0, data.Length)

        Catch ex As Exception
            _ErrorMessage = ex.Message
            Return False
        End Try

        Return True
    End Function

    Public Function Read(ByRef message As String) As Boolean
        Try
            Dim messageBuf As New StringBuilder

            Dim swTimeout As New Stopwatch
            swTimeout.Start()

            Do
                Dim data(&HFFFF) As Byte

                '読める分だけ読出し
                m_stream.Read(data, 0, data.Length)

                Dim strDataBuf As String = Encoding.UTF8.GetString(data)
                messageBuf.Append(strDataBuf)

                'デリミタ有無チェック
                If strDataBuf.Contains(Delimiter) Then
                    Exit Do
                End If

                'タイムアウト
                If swTimeout.Elapsed.TotalMilliseconds > 5000 Then
                    Throw New Exception("受信タイムアウト 受信済みデータ:" & messageBuf.ToString())
                End If
            Loop

            message = messageBuf.ToString

        Catch ex As Exception
            _ErrorMessage = ex.Message
            Return False
        End Try

        Return True
    End Function
End Class
