Public Class Enemy

    Private Enemygraphic As PictureBox
    Private MovementSpeed As Integer
    Private Health As Integer
    Private CoinsDropped As Integer
    Private IsDead As Boolean

    Public Sub New(Graphic As PictureBox, MovementSpeed As Integer, Health As Integer, CoinsDropped As Integer)

        Enemygraphic = Graphic

        With Me

            .MovementSpeed = MovementSpeed
            .Health = Health
            .CoinsDropped = CoinsDropped

        End With

    End Sub

    'Method to make the enemy move across the path
    'Each condition adjusts the enemygraphic to the number of pixels equal to the movement speed in the given direction 
    'Conditions are used to make the enemy move in one direction until it reaches a certain point

    Public Sub MoveEnemy()

        If Enemygraphic.Location.X > -810 And Enemygraphic.Location.X < 426 Then
            Enemygraphic.Left += MovementSpeed
        End If

        If Enemygraphic.Location.X > 425 And Enemygraphic.Location.Y > 92 Then
            Enemygraphic.Top -= MovementSpeed
        End If

        If Enemygraphic.Location.Y <= 92 Then
            Enemygraphic.Left += MovementSpeed
        End If

    End Sub

    'Method used for when an enemy reaches the players base
    'If there is a collision it will remove the enemy from the form, decrease player lives by 1, increase enemieskilledinwave by 1
    'IsDead attribute is set to true to prevent the same enemy from triggering other enemy death conditions after it has died

    Public Sub EnemyReachedBase()

        If Enemygraphic.Bounds.IntersectsWith(CitadelClash.PlayerBase.Bounds) And IsDead = False Then

            Enemygraphic.Top -= 1000
            With CitadelClash
                .setLives(1)
                .setEnemiesKilledInWave(1)
            End With
            IsDead = True
        End If

    End Sub

    Public Function getEnemyGraphic()

        Return Enemygraphic

    End Function

    Public Sub setHealth(DamageRecieved As Integer)

        Health -= DamageRecieved

    End Sub

    Public Function getHealth()

        Return Health

    End Function

    Public Sub setIsdead(state As Boolean)

        IsDead = state

    End Sub
    Public Function getIsDead()

        Return IsDead

    End Function

    Public Function getCoinsDropped()

        Return CoinsDropped

    End Function

End Class
