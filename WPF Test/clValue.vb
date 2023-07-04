Imports System.Runtime.Intrinsics.X86

'DOCU:
'Numbers should only need 5 chars to display enormly large numbers
'The idea is to work with signs like k for 1.000 and m for million
'Afterwards the Chars A, B, ... and so on represent an additional 1.000
'E.g.: 1.495 = 1.49k       3.000.000 = 3.00m        345.123 = 345k

Public Class clValue
    Public v As Single 'Decimal number to store the relevant value
    Public d As Byte   'Byte to store number of 1.000 places
    'e.g.: z=12,345,678 -> v=12.345678; d=2 -> z = v * 1.000^d

#Region "-------------------------- Region: Constructor"

    Public Sub New(ByVal _v As clValue)
        v = _v.v
        d = _v.d
        Normalize_V()
    End Sub
    Public Sub New(Optional ByVal _v As Single = 0, Optional _d As Byte = 0)
        v = _v
        d = _d
        Normalize_V()
    End Sub

#End Region

#Region "-------------------------- Region: Returns"

    Public Function Write_V() As String
        Dim temp As String
        If v >= 100 Then
            temp = " " & v.ToString("000", System.Globalization.CultureInfo.InvariantCulture)
        ElseIf v >= 10 Then
            temp = v.ToString("00.0", System.Globalization.CultureInfo.InvariantCulture)
        Else
            temp = v.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)
        End If
        'If d <> 0 Then temp = temp & Get_Sign()
        temp = temp & Get_Sign()
        Return temp
    End Function
    Public Function Get_Dbl() As Double
        Return CDbl(v * (1000 ^ d))
    End Function
    Public Function Get_Int() As Integer
        Return CInt(v * (1000 ^ d))
    End Function

#End Region

#Region "-------------------------- Region: Operator"

    Public Shared Operator +(ByVal _a As clValue, _b As clValue) As clValue
        Dim r As clValue = New clValue(_a.v, _a.d)
        r.Add(_b)
        Return r
    End Operator

#End Region

#Region "-------------------------- Region: Public Subs"
    Public Sub Set_To(ByVal _v As clValue)
        v = _v.v
        d = _v.d
    End Sub
    Public Sub Add(ByVal _v As clValue)
        If d = _v.d Then
            v += _v.v
        ElseIf d > _v.d Then
            v += CSng(_v.d / (1000 ^ (d - _v.d)))
        ElseIf d < _v.d Then
            v = _v.v + CSng(v / (1000 ^ (_v.d - d)))
            d = _v.d
        End If
        Normalize_V()
    End Sub
    Public Sub Multiply(ByVal _v As clValue)
        v *= _v.v
        d += _v.d
        Normalize_V()
    End Sub

    Public Sub Clear()
        v = 0
        d = 0
    End Sub
#End Region

#Region "-------------------------- Region: Private Subs"
    Private Sub Normalize_V()
        If v / 1000 > 1 Then
            v /= 1000
            d += 1
            Normalize_V()
        End If
    End Sub
    Private Function Get_Sign() As Char
        Select Case d
            Case 0
                Return " "
            Case 1
                Return "k"
            Case 2
                Return "m"
            Case Else
                Return Chr(62 + d)  '65=A, 66=B usw.
        End Select
    End Function

#End Region

End Class
