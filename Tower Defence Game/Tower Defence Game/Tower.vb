Public Class Tower


    Private TowerGraphic As PictureBox
    Private TargetEnemy As Enemy
    Private Damage As Integer
    Private Range As Integer
    Private TowerUI As Panel
    Private LblBuffPrice As Label
    Private CurrentUpgrade As Integer
    Private BuffPrice As Integer
    Private UpgradeSlots(2) As PictureBox

    Public Sub New(Graphic As PictureBox, TowerUI As Panel, LblBuffPrice As Label, UpgradeSlots1 As PictureBox, UpgradeSlots2 As PictureBox, UpgradeSlots3 As PictureBox, Range As Integer, Damage As Integer, BuffPrice As Integer)

        TowerGraphic = Graphic
        UpgradeSlots(0) = UpgradeSlots1
        UpgradeSlots(1) = UpgradeSlots2
        UpgradeSlots(2) = UpgradeSlots3

        With Me

            .TowerUI = TowerUI
            .LblBuffPrice = LblBuffPrice
            .LblBuffPrice.Text = BuffPrice & " COINS"
            .Range = Range
            .Damage = Damage
            .BuffPrice = BuffPrice

        End With

    End Sub

    'Method used to find the next target for a Tower
    'First if statement is used to check if the Tower already has a target, and if this target is no longer in range, then it should no longer be its target
    'Second if statement is used to assign a Towers target to an Enemy, which is in its range, if it currently doesnt't have a target
    'As soon as it finds a target for the Tower, it assigns it as its target, exits for since it is no longer necessary

    Public Sub FindTarget()

        If TargetEnemy IsNot Nothing Then
            If EnemyIsInRange(Me, TargetEnemy) = False Then

                TargetEnemy = Nothing

            End If
        End If

        If TargetEnemy Is Nothing Then

            For Each Enemy As Enemy In CitadelClash.getEnemies

                If EnemyIsInRange(Me, Enemy) Then

                    TargetEnemy = Enemy
                    Exit For

                End If

            Next

        End If

    End Sub

    'Method used to shoot the Towers target
    'First for each loop checks every other Tower to check if its targets health is already 0, if so it sets its target to nothing, this prevents the enemy death condition from running for an enemy that is already dead
    'If statement is used to check if the Tower has a target, if so it then reduces the targets health by 1
    'Then if the targets health reaches 0 it incerases the enemieskilledinwave and totalenemieskilled by 1
    'Increases the Players coins by the amount recieved from the Enemy and increases total coins earned by the amount recieved from the Enemy
    'targetenemies associated Enemy object isdead attribute is set to true
    'target Enemy is set to nothing

    Public Sub shootTarget()

        With CitadelClash

            For Each otherTower As Tower In .getTowers

                If otherTower.TargetEnemy IsNot Nothing AndAlso otherTower.TargetEnemy.getHealth <= 0 Then

                    otherTower.TargetEnemy = Nothing

                End If

            Next

            If TargetEnemy IsNot Nothing Then

                TargetEnemy.setHealth(Damage)


                If TargetEnemy.getHealth <= 0 And TargetEnemy.getIsDead = False Then

                    TargetEnemy.getEnemyGraphic.Top -= 1000
                    .setEnemiesKilledInWave()
                    .setTotalEnemiesKilled()

                    If .getWaveEnded = False Then
                        .setCoins(TargetEnemy.getCoinsDropped)
                        .settotalCoinsEarned(TargetEnemy.getCoinsDropped)
                    End If

                    TargetEnemy.setIsdead(True)
                    TargetEnemy = Nothing

                End If
            End If

        End With

    End Sub

    'Towers will shoot any enemy that is in an area of a circle that surrounds the tower
    'the radius of this circle is equal to the towers range
    'Therefore if the distance between a Tower and an Enemy is less than the range, it must be in this valid area to a target
    'Function used to calculate the distance between a given Tower and Enemy

    Public Function EnemyIsInRange(Tower As Tower, Enemy As Enemy) As Boolean

        Dim p As New Point(18, 16)

        Dim towerCentreX As Integer = Tower.TowerGraphic.Location.X + p.X
        Dim towerCentreY As Integer = Tower.TowerGraphic.Location.Y + p.Y
        Dim enemyCentreX As Integer = Enemy.getEnemyGraphic.Location.X + Enemy.getEnemyGraphic.Width \ 2
        Dim enemyCentreY As Integer = Enemy.getEnemyGraphic.Location.Y + Enemy.getEnemyGraphic.Height \ 2

        Dim distance As Double = Math.Sqrt((towerCentreX - enemyCentreX) ^ 2 + (towerCentreY - enemyCentreY) ^ 2)


        Return Enemy.getEnemyGraphic.Location.X >= 15 And
    distance <= Range

    End Function


    Public Function getTowerGraphic()

        Return TowerGraphic

    End Function

    Public Function getTowerUI()

        Return TowerUI

    End Function

    Public Sub setLblBuffPrice(Index As Integer)

        LblBuffPrice.Text = BuffPrice & " COINS"

        If Index = 3 Then
            LblBuffPrice.Text = "MAXED"
        End If

    End Sub

    Public Function getUpgradeSlots(Index As Integer)

        Return UpgradeSlots(Index)

    End Function

    Public Sub setDamage(Newdamage As Integer)

        Damage = Newdamage

    End Sub

    Public Sub setBuffPrice(NewPrice As Integer)

        BuffPrice = NewPrice

    End Sub

    Public Function getBuffprice()

        Return BuffPrice

    End Function

    Public Sub setcurrentUpgrade()

        CurrentUpgrade += 1

    End Sub

    Public Function getcurrentUpgrade()

        Return CurrentUpgrade

    End Function

End Class
