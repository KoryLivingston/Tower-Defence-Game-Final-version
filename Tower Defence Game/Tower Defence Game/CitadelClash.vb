Public Class CitadelClash

    'ATTRIBUTES USED FOR ENEMIES


    'EnemiesKilledInWave will be incremented from 0 for every Enemy that has been killed
    'Totalenemiesinwave will be assigned to the value equal to the number of enemies created for that wave
    'WaveEnded is used as a boolean flag to prevent the condition from running while the next wave has yet to begin
    'These variables are used to check if a wave has ended

    Private EnemiesKilledInWave As Integer
    Private TotalEnemiesInWave As Integer
    Private WaveEnded As Boolean

    'ARRAYS OF OBJECTS for Enemy and Picturebox, to be used for each enemy

    Private Enemies() As Enemy
    Private PicEnemies() As PictureBox

    'additionalEnemies is increased by 2 after a wave has ended, used to increase the amount of enemies spawned for the next wave
    'Enemynum is used to hold the index of the next Enemy to be spawned as well is to check if every Enemy has been spawned
    'LastEnemy holds the last Enemy to be spawned, used to check if this Enemy has died and to then spawn the remaining amount of enemies, in groups of 10 for that wave 2 seconds later

    Private additionalEnemies As Integer
    Private EnemyNum As Integer
    Private LastEnemy As Enemy


    'ATTRIBUTES USED FOR TOWERS


    'ARRAYS OF OBJECTS for Tower and Picturebox, to be used for each Tower
    'INSTANTIATES a List to hold the objects of all Towers, to allow Tower objects to be added or removed when created or sold

    Private currentTowers As New List(Of Tower)
    Private Towers() As Tower
    Private PicTower() As PictureBox

    'ARRAYS OF OBJECTS of Panel, Label and Picturebox for the UI for each Tower

    Private TowerUI() As Panel
    Private LblBuffPrice() As Label
    Private UpgradeSlots1() As PictureBox
    Private UpgradeSlots2() As PictureBox
    Private UpgradeSlots3() As PictureBox

    'Towercount is used to redclare the arrays to hold the next position of the next Tower to be created
    'Towerplacing is used to determine whether the player is placing a Tower and to run certain code while doing so

    Private TowerCount As Integer
    Private TowerPlacing As Boolean

    'ClickedTower holds the Tower object that has been clicked, this will then be used in the code involving the tower UI
    'Index holds the position of this clicked Tower in the list, so if it is sold it can be removed from the list

    Private clickedTower As Tower
    Private Index As Integer


    'PLAYER STATS

    Private Lives As Integer = 10
    Private Coins As Integer = 60
    Private Wave As Integer = 1



    'ATTRIBUTES USED FOR THE LEADERBOARD


    'Arrays to hold the player names and wavesReached of the 5 players in the leaderboard, with 6th position being nothing, which will be assigned to the Player Name and Wavesreached of the current player playing the game
    'This will allow this new player to be insertion sorted into the leaderboard while removing one from it as well
    'When the leaderboard will be displayed on the screen it will only display the first 5 Player Names and their associated Waves Reached
    'The arrrays have been set to initial values to hold a starting leaderboard.

    Private PlayerNames() As String = {"QSD", "BMV", "AJS", "123", "HJT", Nothing}
    Private WavesReached() As Integer = {"13", "10", "8", "12", "5", Nothing}

    'Arrays of label to hold the associated Playername and Wavesreached to allow this information to be displayed on screen

    Private NameLabels(4) As Label
    Private WaveReachedLabels(4) As Label

    'UserName contains the name of the player which is currently player, used to add this name to its assoiated position in the leaderboard if they meet the criteria

    Private UserName As String

    'Information gathered while playing the game which will be used in the Database

    Private TotalEnemiesKilled As Integer
    Private TotalCoinsEarned As Integer


    Private Sub StartButton_Click(sender As Object, e As EventArgs) Handles StartButton.Click

        'Once the startbutton is clicked it:
        'Spawns the enemies for wave 1
        'Initializes the labels to be used in the leaderboard
        'Loads the game UI
        'Starts the games timers

        WaveSpawn()
        InitializeLabels()
        LoadUI()
        startGameTimers()


    End Sub

    Private Sub GameLogic_Tick(sender As Object, e As EventArgs) Handles GameLogic.Tick

        'Updates Player Stats Labels, to display new values on the screen while playing the game

        LblLives.Text = "LIVES " & Lives
        LblCoins.Text = "COINS " & Coins
        LblWave.Text = "WAVE " & Wave

        'Checks if a wave has ended by comparing the enemieskilled to the totalenemies from that wave
        'Waveended is used to prevent the condition from running again until the next wave has been called from pressing the next wave button
        'Starts the wavecompletionUI timer to simulate the label appearing for only 2 seconds, and to show the Nextwavebutton once this has been done

        If WaveEnded = False And EnemiesKilledInWave = TotalEnemiesInWave Then

            LblWaveCompleteD.Show()
            WaveCompletionUI.Start()
            WaveEnded = True

        End If

        'Once the Player Lives reaches 0 or they complete the final wave it ends the game
        'if wave is equal to 26 they have completed the final wave 25 resulting in the game ending

        If Lives = 0 Or Wave = 26 Then
            Endgame()
        End If



        'Moves each enemy along the path and checks if they have reached the players base at the other end

        For counter = 0 To TotalEnemiesInWave - 1

            Enemies(counter).MoveEnemy()
            Enemies(counter).EnemyReachedBase()

        Next

        'LastEnemy will only be assigned an Enemy object if there are remaining enemies to be spawned
        'Checks if the Last Enemy has died so that the next group of enemies can be brought in, with a maximum of 10 per group

        If LastEnemy IsNot Nothing AndAlso LastEnemy.getIsDead = True Then
            NextSpawnDelay.Start()
            LastEnemy = Nothing
        End If



        'Updates Tower Cap Label, to display new values on the screen while playing the game

        LblTowerCap.Text = "Tower Capacity" & " " & TowerCount & "/25"



        'Finds every Tower a target
        'Updates its Buff Price label, so that it holds the price of the next available upgrade for that tower

        For counter = 0 To TowerCount - 1

            currentTowers(counter).FindTarget()

            currentTowers(counter).setLblBuffPrice(currentTowers(counter).getcurrentUpgrade)

        Next


        'Adjust the Tower indicators position, so that its centre is under the users Cursor at all times
        'Using a point object, the towerindicator location is set to the Clients location of the cursor sbtracted by p
        'Tower Indicator will only be shown while placing a Tower


        Dim p As New Point(18, 16)

        TowerIndicator.Location = New Point(PointToClient(Cursor.Position) - p)

        'If the player is currently placing a Tower then show the Tower indicator and adjust its opacity

        If TowerPlacing = True Then

            TowerIndicator.Show()
            TowerIndicator.BringToFront()
            TowerIndicator.BackColor = Color.FromArgb(200, TowerIndicator.BackColor)

        End If



        'UI brought to the front when in use to prevent the Tower indicator from overlapping it

        LblWavecompleted.BringToFront()
        NextWaveButton.BringToFront()
        CancelPlacing.BringToFront()

    End Sub


    'Used to create each Enemy with their associated Picturebox attributes and Enemy attributes of that wave

    Public Sub CreateEnemies(NumberOfEnemies As Integer, EnemySizeX As Integer, EnemySizeY As Integer, EnemyColour As Color, movementSpeed As Integer, Health As Integer, CoinsDropped As Integer)

        'Redeclares the arrays to hold the amount of Enemies needed for that wave

        ReDim Enemies(NumberOfEnemies - 1)
        ReDim PicEnemies(NumberOfEnemies - 1)

        'For every Enemy a picturebox is created with its associated attributes, and then an Enemy object is instantiated to hold this new Enemy with its Picturebox
        'TotalEnemeisInWave is increased to be used in conditions throught the Program

        For counter = 0 To NumberOfEnemies - 1

            PicEnemies(counter) = New PictureBox With {
                .Location = New Point(-1000, -100),
                .Size = New Size(EnemySizeX, EnemySizeY),
                .BackColor = EnemyColour
            }

            Controls.Add(PicEnemies(counter))
            PicEnemies(counter).BringToFront()


            Enemies(counter) = New Enemy(PicEnemies(counter), movementSpeed, Health, CoinsDropped)
            TotalEnemiesInWave += 1

        Next

    End Sub

    'Timer starts and has an interval of 2 seconds
    'Once it has reached 2 seconds it displays the nextwavebutton to allow the next wave to be called, and hides the label and stops the timer

    Private Sub WaveCompletionUI_Tick(sender As Object, e As EventArgs) Handles WaveCompletionUI.Tick

        LblWavecompleted.Hide()
        WaveCompletionUI.Stop()
        NextWaveButton.Show()
        NextWaveButton.BringToFront()

    End Sub

    'Occurs if the Nextwavebutton is pressed:
    'Hides the button and increases the Wave by one
    'Checks if the Wave is now any of the Waves that brings in a new type of Enemy, if so, Additionalenemies should be set to 0 for this new enemy type
    'Additionalenemies is increased by 2 to allow a new total of Enemies to be brought into the next Wave
    'Wavespawn() is called to spawn the next Wave

    Private Sub NextWaveButton_Click(sender As Object, e As EventArgs) Handles NextWaveButton.Click

        NextWaveButton.Hide()

        Wave += 1

        If Wave = 8 Or Wave = 16 Or Wave = 26 Then
            additionalEnemies = 0
        End If

        additionalEnemies += 2
        WaveSpawn()

    End Sub

    Public Sub WaveSpawn()

        'If else if statements checks which Wave it currently is and to create its associated Enemies
        'Each time the Game is initialized to reset the Arrays and logic variables used during each Wave, to allow them to be used successfully in the next wave.

        If Wave = 1 Then

            CreateEnemies(3, 30, 30, Color.Green, 1, 4, 5)


            For counter = 0 To TotalEnemiesInWave - 1
                Enemies(counter).getEnemyGraphic.Location = New Point(-650 - (counter * 50), 278)
            Next

        ElseIf Wave >= 2 And Wave <= 7 Then

            InitializeGame()
            CreateEnemies(3 + additionalEnemies, 30, 30, Color.Green, 1, 4, 5)

        ElseIf Wave >= 8 And Wave <= 15 Then

            InitializeGame()
            CreateEnemies(10 + additionalEnemies, 30, 30, Color.DarkRed, 2, 6, 6)

        ElseIf Wave >= 16 And Wave <= 25 Then

            InitializeGame()
            CreateEnemies(15 + additionalEnemies, 30, 30, Color.Gray, 3, 8, 7)

        End If

        'If the wave is no longer wave 1 then for the first ten Enemies adjust their positions to be closer to the Enemybase so that they get into the game faster
        'At any point if all the Enemies have already been brought into the Game exit sub

        For counter = 0 To 9

            If Wave <> 1 AndAlso EnemyNum < TotalEnemiesInWave Then
                Enemies(EnemyNum).getEnemyGraphic.Location = New Point(EnemyBase.Location.X - 100 - (counter * 50), 278)
                EnemyNum += 1
            Else
                Exit Sub
            End If

        Next

        'Reaches this line if there are more Enemies to be brought into the Game.
        'Sets the LastEnemy to the Last Enemy to be brought into the Game to be used in the condition to bring the next group of Enemies in


        LastEnemy = Enemies(EnemyNum - 1)

    End Sub

    'Spawns in another 10 Enemies, If at any point enemynum is no longer less than totalenemiesinwave then stop the timer, as this means all the Enemies of that wave have been brought it.

    Private Sub EnemySpawnTimer_Tick(sender As Object, e As EventArgs) Handles NextSpawnDelay.Tick


        For counter = 0 To 9
            If EnemyNum < TotalEnemiesInWave Then
                Enemies(EnemyNum).getEnemyGraphic.Location = New Point(EnemyBase.Location.X - 100 - (counter * 50), 278)
                EnemyNum += 1
            Else
                NextSpawnDelay.Stop()
            End If
        Next

        'Reaches this line if their are Enemies left over
        'LastEnemy is assigned to the last enemy to be brought in
        'This allows the timer to be recalled again from the condition in GameLogic for the next 10 Enemies to be brought into the Game once the last Enemy has died

        LastEnemy = Enemies(EnemyNum - 1)
        NextSpawnDelay.Stop()

    End Sub


    'If they have enough coins and are currently not placing a Tower as well as the total Towers placed has yet to reach 25 and the Game hasn't ended
    'Then set Towerplacing to true and show the cancel button

    Private Sub BuyTower_Click(sender As Object, e As EventArgs) Handles BuyTower.Click

        If Coins >= 30 And TowerPlacing = False AndAlso TowerCount < 25 AndAlso Lives > 0 Then

            TowerPlacing = True
            CancelPlacing.Show()

        End If

    End Sub


    Private Sub TowerIndicator_Click(sender As Object, e As EventArgs) Handles TowerIndicator.Click

        Dim p As New Point(18, 16)


        'Checks if the Tower Indicator is not in collision with any Picturebox to ensure that the spawn location is valid
        'If the location is valid then it will Hide the UI used during Tower placing and create a Tower to be placed at this new set position
        'Each array is redclared to towercount index to allow a new Tower to be created with its associated UI
        'Once each object for a Tower has been created, it INSTANTIATES a new tower object for this new Tower

        If TowerPlacing = True Then


            For Each PictureBox As PictureBox In Controls.OfType(Of PictureBox)()

                If PictureBox IsNot TowerIndicator AndAlso TowerIndicator.Bounds.IntersectsWith(PictureBox.Bounds) Then
                    Exit Sub
                End If
            Next

            TowerIndicator.Hide()
            CancelPlacing.Hide()

            'Creates a new Picturebox for the Tower

            ReDim Preserve PicTower(TowerCount)


            PicTower(TowerCount) = New PictureBox With {
    .Size = New Size(37, 33),
    .BackColor = Color.CadetBlue,
    .Name = "PicTower" & TowerCount.ToString,
    .Location = PointToClient(Cursor.Position) - p
                    }

            Controls.Add(PicTower(TowerCount))
            PicTower(TowerCount).BringToFront()


            'Adds the picturebox as handler to the click event, therefore if the newly created picturebox is clicked it will run the sub routine

            AddHandler PicTower(TowerCount).Click, AddressOf Tower_Click


            'Creates a new UI for the Tower

            ReDim Preserve TowerUI(TowerCount)

            TowerUI(TowerCount) = New Panel With {
                .Size = New Size(108, 164),
                .BackColor = Color.FromArgb(130, TowerPanel.BackColor),
                .Location = New Point(-1000, 100)
                }

            Controls.Add(TowerUI(TowerCount))

            'Creates a new Buff Price for the Tower

            ReDim Preserve LblBuffPrice(TowerCount)

            LblBuffPrice(TowerCount) = New Label With {
                .Font = New Font("Agency FB", 10, FontStyle.Bold),
                .ForeColor = Color.White,
                     .TextAlign = 2,
                .Size = New Size(82, 17),
                .BackColor = Color.FromArgb(130, TowerPanel.BackColor),
                .Location = New Point(14, 55)
            }

            'Remaining UI will be added to the TowerUI controls to be used on the newly created panel

            TowerUI(TowerCount).Controls.Add(LblBuffPrice(TowerCount))

            'Creates upgrade slots Pictureboxes for the new tOWER

            ReDim Preserve UpgradeSlots1(TowerCount)

            UpgradeSlots1(TowerCount) = New PictureBox With {
                .Size = New Size(21, 11),
                .BackColor = Color.White,
                .Location = New Point(14, 5)
                }


            TowerUI(TowerCount).Controls.Add(UpgradeSlots1(TowerCount))


            ReDim Preserve UpgradeSlots2(TowerCount)

            UpgradeSlots2(TowerCount) = New PictureBox With {
                .Size = New Size(21, 11),
                .BackColor = Color.White,
                .Location = New Point(45, 5)
                }


            TowerUI(TowerCount).Controls.Add(UpgradeSlots2(TowerCount))



            ReDim Preserve UpgradeSlots3(TowerCount)

            UpgradeSlots3(TowerCount) = New PictureBox With {
                .Size = New Size(21, 11),
                .BackColor = Color.White,
                .Location = New Point(75, 5)
                }


            TowerUI(TowerCount).Controls.Add(UpgradeSlots3(TowerCount))

            'Creates a new Tower object at index TowerCount in the Towers array of objects

            ReDim Preserve Towers(TowerCount)
            Towers(TowerCount) = New Tower(PicTower(TowerCount), TowerUI(TowerCount), LblBuffPrice(TowerCount), UpgradeSlots1(TowerCount), UpgradeSlots2(TowerCount), UpgradeSlots3(TowerCount), 180, 1, 100)
            currentTowers.Add(Towers(TowerCount))

            Coins -= 30
            TowerCount += 1

            TowerPlacing = False

        End If


    End Sub


    'Stops the player from placing a Tower and Hides the relevant UI

    Private Sub CancelPlacing_Click(sender As Object, e As EventArgs) Handles CancelPlacing.Click

        TowerPlacing = False
        TowerIndicator.Hide()
        CancelPlacing.Hide()

    End Sub

    'Timer that runs every second to deal damage to its target every second

    Private Sub TowerShooting_Tick(sender As Object, e As EventArgs) Handles TowerShooting.Tick

        For counter = 0 To TowerCount - 1
            currentTowers(counter).shootTarget()
        Next

    End Sub



    Public Sub Tower_Click(sender As Object, e As EventArgs)

        'If clickedTower has already been declared then this must be the second time that the same tower was clicked
        'First time brings up the UI, and this second time will remove it

        If clickedTower IsNot Nothing Then
            clickedTower.getTowerUI.Hide()
            clickedTower = Nothing
            Exit Sub
        End If

        'Find the tower object that has been clicked and set it to clickedtower

        For counter = 0 To TowerCount - 1

            If sender Is currentTowers(counter).getTowerGraphic Then
                clickedTower = currentTowers(counter)
                Index = counter
                Exit For
            End If

        Next

        'Once clickedtower has been assigned a value then show the towers UI

        If clickedTower IsNot Nothing Then

            With clickedTower.getTowerUI

                .Show()
                .bringtofront()
                clickedTower.getTowerGraphic.bringtofront()
                .Location = New Point(clickedTower.getTowerGraphic.location.x - 36, clickedTower.getTowerGraphic.location.y - 80)
                .controls.add(SellButton)
                .controls.add(DamageBuffButton)

            End With

            SellButton.Location = New Point(14, 130)
            DamageBuffButton.Location = New Point(14, 22)

        End If

    End Sub

    Private Sub SellButton_Click(sender As Object, e As EventArgs) Handles SellButton.Click

        'If the sell button is clicked then increase the players coins by 20 and delete the tower and hide the towers UI

        Coins += 20


        Controls.Remove(currentTowers(Index).getTowerGraphic)
        currentTowers.RemoveAt((Index))
        TowerCount -= 1


        clickedTower.getTowerUI.Hide()
        clickedTower = Nothing

    End Sub

    Private Sub DamageBuffbutton_Click(sender As Object, e As EventArgs) Handles DamageBuffButton.Click

        'If the damageBuffbutton is clicked and the player has enough coins and the tower is currently not fully upgraded
        'Then upgrade the tower to the next availble upgrading, increasing its damage by 1 with every succesive one, changing the upgradeslot of the associated upgrade to green to indicate it has been bought
        'deduct the buffprice from the coins and set the new buffprice for the next upgrade as well increasing currentUpgrade by 1 so that the next upgrade can be bought


        With clickedTower
            If Coins >= .getBuffprice And .getcurrentUpgrade <= 2 Then


                .setDamage(2 + .getcurrentUpgrade)

                .getUpgradeSlots(.getcurrentUpgrade).BackColor = Color.Green
                Coins -= .getBuffprice
                .setBuffPrice(250 + (.getcurrentUpgrade * 250))
                .setcurrentUpgrade()

            End If
        End With

    End Sub

    'Used to reset the game to a state to allow the next wave to use the games logic

    Public Sub InitializeGame()

        'Remoevs pictureboxes of previous wave

        For counter = 0 To TotalEnemiesInWave - 1
            Controls.Remove(Enemies(counter).getEnemyGraphic)
            Enemies(counter).getEnemyGraphic.Dispose()
        Next

        'Sets arrays to nothing, logic variables to 0 or false and stops the nextspawndelay timer

        Enemies = Nothing
        PicEnemies = Nothing
        TotalEnemiesInWave = 0
        EnemiesKilledInWave = 0
        WaveEnded = False
        EnemyNum = 0
        LastEnemy = Nothing
        NextSpawnDelay.Stop()

    End Sub

    'Runs if the player dies or completes the last wave
    'Stops the game from running by stopping the timers and displays the game over screen while starting the timer for the leaderboard to be shown 2.5 seconds later

    Public Sub Endgame()

        GameLogic.Stop()
        TowerShooting.Stop()

        LblGameOver.Show()
        LblGameOver.BringToFront()

        LeaderBoardDelay.Start()

    End Sub

    Private Sub LeaderBoardDelay_Tick(sender As Object, e As EventArgs) Handles LeaderBoardDelay.Tick

        'Hides Game over screen
        'Moves the leaderboard panel to cover the screen and stops the timer to prevent it from being shown once the next game has started


        LblGameOver.Hide()
        LeaderBoardPanel.Location = New Point(0, 0)
        LeaderBoardPanel.Show()
        LeaderBoardPanel.BringToFront()
        LeaderBoardDelay.Stop()

    End Sub


    Private Sub newName_KeyDown(sender As Object, e As KeyEventArgs) Handles newName.KeyDown

        'If the key pressed down was enter and the name entered is valid then it hides the previous leaderboard UI
        'Declares the new name and waves reached to be held in the 6th position of the arrays for playernames and waves reached

        If e.KeyCode = Keys.Enter And newName.Text.Length = 3 Then

            newName.Hide()
            LblEnterUsername.Hide()
            UserName = newName.Text
            PlayerNames(5) = UserName
            WavesReached(5) = Wave

            'INSERTION SORTS the leaderboard with respect to waves reached

            For counter = 1 To 5

                Dim tempName = PlayerNames(counter)
                Dim tempWave = WavesReached(counter)
                Dim index = counter

                While index > 0 AndAlso tempWave > WavesReached(index - 1)

                    WavesReached(index) = WavesReached(index - 1)
                    PlayerNames(index) = PlayerNames(index - 1)

                    index = index - 1
                End While

                WavesReached(index) = tempWave
                PlayerNames(index) = tempName

            Next

            'Sets each label to their associated playerName and waves reached for the first 5 positions and shows the leaderboard

            For counter = 0 To 4

                NameLabels(counter).Text = PlayerNames(counter)
                NameLabels(counter).Show()

                WaveReachedLabels(counter).Text = WavesReached(counter)
                WaveReachedLabels(counter).Show()

            Next

            'Displays leaderbaord UI

            UpdateTableButton.Show()
            LblPlayerNamesHeading.Show()
            LblWavesReachedHeading.Show()


        End If

    End Sub

    Private Sub RetryButton_Click(sender As Object, e As EventArgs) Handles RetryButton.Click

        'Hides Leaderboard UI and resets the text box and shows the tex box for next use

        newName.Show()
        LblEnterUsername.Show()
        LblPlayerNamesHeading.Hide()
        LblWavesReachedHeading.Hide()
        UpdateTableButton.Hide()
        LeaderBoardPanel.Hide()
        newName.Text = Nothing

        For counter = 0 To 4
            NameLabels(counter).Hide()
            WaveReachedLabels(counter).Hide()
        Next

        'Resets player stats

        Lives = 10
        Coins = 60
        Wave = 1

        'Resets the game for enemies

        additionalEnemies = 0
        InitializeGame()

        'Resets the game for towers

        For counter = 0 To TowerCount - 1
            Controls.Remove(currentTowers(counter).getTowerGraphic)
        Next

        Towers = Nothing
        PicTower = Nothing
        TowerUI = Nothing
        LblBuffPrice = Nothing
        UpgradeSlots1 = Nothing
        UpgradeSlots2 = Nothing
        UpgradeSlots3 = Nothing
        currentTowers.Clear()
        TowerCount = 0
        TowerPlacing = False


        startGameTimers()

        'Spawns wave 1 again

        WaveSpawn()

    End Sub

    Private Sub QuitButton_Click(sender As Object, e As EventArgs) Handles QuitButton.Click

        Close()

    End Sub

    Public Sub startGameTimers()

        GameLogic.Start()

        TowerShooting.Start()

    End Sub

    Public Sub LoadUI()

        StartButton.Hide()
        QuitButton.Hide()

        LblLives.Show()
        LblCoins.Show()
        LblWave.Show()

        TowerPanel.Show()
        LblTowerCost.Show()
        BuyTower.Show()
        LblTowerCap.Show()
        TowerPanel.BackColor = Color.FromArgb(130, TowerPanel.BackColor)
        LblTowerCost.BackColor = Color.FromArgb(130, TowerPanel.BackColor)
        LblTowerCap.BackColor = Color.FromArgb(130, TowerPanel.BackColor)

    End Sub

    Public Sub InitializeLabels()

        NameLabels(0) = LblName1
        NameLabels(1) = LblName2
        NameLabels(2) = LblName3
        NameLabels(3) = LblName4
        NameLabels(4) = LblName5

        WaveReachedLabels(0) = LblWavesReached1
        WaveReachedLabels(1) = LblWavesReached2
        WaveReachedLabels(2) = LblWavesReached3
        WaveReachedLabels(3) = LblWavesReached4
        WaveReachedLabels(4) = LblWavesReached5

        For counter = 0 To 4

            NameLabels(counter).Text = PlayerNames(counter)
            WaveReachedLabels(counter).Text = WavesReached(counter)

        Next

    End Sub

    Public Sub setLives()

        Lives -= 1

    End Sub

    Public Sub setCoins(CoinsEarned As Integer)

        Coins += CoinsEarned

    End Sub

    Public Sub settotalCoinsEarned(CoinsEarned As Integer)

        TotalCoinsEarned += CoinsEarned
    End Sub

    Public Sub setEnemiesKilledInWave()

        EnemiesKilledInWave += 1

    End Sub

    Public Sub setTotalEnemiesKilled()

        TotalEnemiesKilled += 1

    End Sub
    Public Function getEnemies()

        Return Enemies

    End Function

    Public Function getTowers()

        Return Towers

    End Function

    Public Function getWaveEnded()

        Return WaveEnded

    End Function

End Class
