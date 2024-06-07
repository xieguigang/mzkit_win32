Public Module Browser

    Public Sub SearchMass(mass As String)
        Call Process.Start($"https://query.biodeep.cn/show?exact_mass={mass}")
    End Sub

    Public Sub SearchFormula(formula As String)
        Call Process.Start($"https://query.biodeep.cn/show?formula={formula}")
    End Sub
End Module
