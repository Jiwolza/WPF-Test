Public Class ucStat

    'features and attributes:
    '--------------------------
    'l = level
    'f = flat       
    'i = increased  
    'm = more       
    'v = value = ( l + f ) * ( sum i ) * ( prod m )


    Private l As clValue
    Private f As clValue
    Private i As clValue
    Private m As clValue
    Private v As clValue

    Public Sub New(_title As String, _l As clValue, _op As Char)
        InitializeComponent()
        lbStat.Content = _title
        lbOperator.Content = _op

        l = New clValue(_l)
        f = New clValue(0)
        i = New clValue(1)
        m = New clValue(1)
        v = New clValue(0)
        v = _l

        lbLcontent.Content = l.Write_V()
        lbFcontent.Content = f.Write_V()
        lbIcontent.Content = i.Write_V()
        lbMcontent.Content = m.Write_V()
        lbStatContent.Content = v.Write_V()
    End Sub

    Public Sub Set_L(_l As clValue)
        l = _l
    End Sub
    Public Sub Add_F(_f As clValue)
        f.Add(_f)
    End Sub
    Public Sub Inc_I(_i As clValue)
        i.Add(_i)
    End Sub
    Public Sub More_M(_m As clValue)
        m.Multiply(_m)
    End Sub

    Public Function Get_Value() As clValue
        v.Set_To(l)
        v.Add(f)
        v.Multiply(i)
        v.Multiply(m)
        lbLcontent.Content = l.Write_V()
        lbFcontent.Content = f.Write_V()
        lbIcontent.Content = i.Write_V()
        lbMcontent.Content = m.Write_V()
        lbStatContent.Content = v.Write_V()
        Return v
    End Function

End Class
