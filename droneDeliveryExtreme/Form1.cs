using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace droneDeliveryExtreme
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //Determines if the player is currently moving in a direction
        bool playerUp = false;
        bool playerDown = false;
        bool playerLeft = false;
        bool playerRight = false;

        //Determines if the player has picked up the parcel
        bool parcelAttached = false;

        //Determines if the parcel is currently in midair
        bool parcelFall = false;

        //The fall speed of the parcel
        int fallSpeed = 1;

        //Count used to speed up the parcel until it reaches max speed, which is 10
        int fallCount = 1;

        //The fall speed of the player
        int playerFallSpeed = 1;

        //Count used to speed up the player when falling until it reaches max speed, which is 10
        int playerFallCount = 1;

        //The fall speed of the enemy
        int enemyFallSpeed = 1;
        //Count used to speed up the enemy when falling until it reaches max speed, which is 10
        int enemyFallCount = 1;

        //The fall speed of the enemy deliverer
        int enemyFallSpeedB = 1;

        //Count used to speed up the enemy deliverer when falling until it reaches max speed, which is 10
        int enemyFallCountB = 1;

        //The current day
        int curDay = 1;

        //The players battery level
        int curBattery = 1000;

        //The players total cash
        int curCash = 0;

        //How many parcels have been dropped in a day
        int parcelsDelivered = 0;

        //How many parcels have been delivered to the correct houses in the day
        double parcelsDeliveredCorrectly = 0;

        //How many parcels have been delivered to the correct houses in each day
        double[] parcelsDeliveredAll = new double[5];

        //How much cash has been earned in each day
        int[] cashEarned = new int[5];

        //The total amount of parcels that need delivering for each day
        int[] totalParcels = { 5, 5, 5, 5, 5 };

        //How much time is currently left in the day
        int timeRemaining = 60;

        //Determines if the drone has run out of battery
        bool deadDrone = false;

        //Determines if the player is shooting
        bool playerShoot = false;

        //Determines if the player is shooting in a direction
        bool playerShootUp = false;
        bool playerShootDown = false;
        bool playerShootLeft = false;
        bool playerShootRight = false;

        //Determines if the enemy is shooting
        bool enemyShoot = false;

        //Determines if the enemy is shooting in a direction
        bool enemyShootUp = false;
        bool enemyShootDown = false;
        bool enemyShootLeft = false;
        bool enemyShootRight = false;

        //Determines if the enemy deliverer is shooting
        bool enemyShootB = false;

        //Determines if the enemy deliverer is shooting in a direction
        bool enemyShootUpB = false;
        bool enemyShootDownB = false;
        bool enemyShootLeftB = false;
        bool enemyShootRightB = false;

        //Determines if the parcel is currently damaged
        bool parcelDamaged = false;

        //Determines if the parcel has been destroyed
        bool parcelBroken = false;

        //Determines if an enemy is low on health
        bool enemyDamaged = false;

        //Determines if an enemy deliverer is low on health
        bool enemyDamagedB = false;

        //Assigns a random value to playerWind, enemyWind, delivererWind, and randomNumber: each having a different seed value
        Random playerRng = new Random(DateTime.Now.Millisecond);
        Random enemyRng = new Random(DateTime.Now.Millisecond + 1);
        Random delivererRng = new Random(DateTime.Now.Millisecond + 2);
        Random houseRng = new Random();

        //Random value from 1-4 that moves drones in a direction depending on the number
        int playerWind;
        int enemyWind;
        int delivererWind;

        //2D Array of what houses that have requested a delivery for each day
        int[,] houseCheck = new int[5, 5]; //[DAY, PARCEL]

        //2D Array of names of the houses that have requested a delivery
        string[,] houseName = new string[5, 5]; //[DAY, PARCEL]

        //The set intensity of the wind for each day, odd numbers are left even numbers are right, the higher the number the higher the intensity
        int[] windDay = new int[5];

        //Determines if the enemy is currently alive
        bool enemyActivea = false;

        //Determines if the enemy deliverer is currently alive
        bool enemyActiveb = false;


        //Determines if the player has delivered to the wrong house
        bool angryHouse = false;

        //Represents what house the player has just delivered to
        int curHouse = 0;

        //Determines if the thrown chair is still midair
        bool houseShoot = false;

        //Represents what road the player is currently on
        int curRoad = 1;

        //Determines if all percels have been delivered and the player has access to the fourth area
        bool extraTime = false;

        //Determines if the player is currently inside the shop
        bool inShop = false;

        //The total points earned across the game
        int totalPts = 0;

        //The total points earned for each day
        int[] dayPts = new int[5];

        //The total rank based on the performance of the player across the game, set as F/E/D/C/B/A/S
        char totalRank;

        //The rank of the player performance across the day, set as F/E/D/C/B/A/S
        char[] dayRank = new char[5];

        //Determines if the player has purchased a specific upgrade
        bool droneUpgraded = false;
        bool batteryUpgraded = false;
        bool bulletsUpgraded = false;

        //Random number of 1-20 which decides if an enemy is going to spawn, and then spawn left or right, 1 being spawn left and 2 being spawn right
        int enemySpawner;

        //The set high score made by the player at the end of the game, can be beaten if the player performs better in the next game
        int highScore = 0;

        //Assigns a random value to enemySpawner
        Random spawnEnemy = new Random();

        //Keeps player and enemies from passing through objects such as the top menu, ground, houses
        bool droneCollisions(int left, int right, int top, int bottom)
        {
            PictureBox[] noTouch = {landGrass, gameHud, house1, house2, house3};
            int numWalls = noTouch.Length;

            for (int count = 0; count < numWalls; count++)
            {
                if ((left < noTouch[count].Right) &&
                    (right > noTouch[count].Left) &&
                    (top < noTouch[count].Bottom) &&
                    (bottom > noTouch[count].Top))
                {
                    return true;
                }
            }
            return false;
        }

        //Allows for parcels to be placed infront of a house, without passing through the ground
        bool droneCollisionsExcHouse(int left, int right, int top, int bottom)
        {
            PictureBox[] noTouch = { landGrass, gameHud};
            int numStuff = noTouch.Length;

            for (int count = 0; count < numStuff; count++)
            {
                if ((left < noTouch[count].Right) &&
                    (right > noTouch[count].Left) &&
                    (top < noTouch[count].Bottom) &&
                    (bottom > noTouch[count].Top))
                {
                    return true;
                }
            }
            return false;
        }

        //Detecting if the player is currently touching the parcel, so they can pick them up
        bool parcelCollisions(int left, int right, int top, int bottom)
        {
            if ((left < playerParcel.Right) &&
                (right > playerParcel.Left) &&
                (top < playerParcel.Bottom) &&
                (bottom > playerParcel.Top))
            {
                return true;
            }
            return false;
        }

        //Detecting if the parcel has landed on the ground, destroying it
        bool groundCollisions(int left, int right, int top, int bottom)
        {
            if ((left < landGrass.Right) &&
                (right > landGrass.Left) &&
                (top < landGrass.Bottom) &&
                (bottom > landGrass.Top))
            {
                return true;
            }
            return false;
        }

        //Detecting if the angry houses thrown furniture has hit the player
        bool playerCollision(int left, int right, int top, int bottom)
        {
            if ((left < playerDrone.Right) &&
                (right > playerDrone.Left) &&
                (top < playerDrone.Bottom) &&
                (bottom > playerDrone.Top))
            {
                return true;
            }
            return false;
        }

        //Detecting if the parcel has been delivered in front of a house
        bool houseCollisions(int left, int right, int top, int bottom)
        {
            PictureBox[] housePic = {house1, house2, house3};
            int numHouse = housePic.Length;
            for (int count = 0; count < numHouse; count++)
            {
                if ((left < housePic[count].Right) &&
                (right > housePic[count].Left) &&
                (top < housePic[count].Bottom) &&
                (bottom > housePic[count].Top))
                {
                    return true;
                }
            }
            return false;
        }

        //Identifies what house the parcel has been placed
        int houseNumCheck(int left, int right, int top, int bottom)
        {
            PictureBox[] housePic = { house1, house2, house3 };
            int numHouse = housePic.Length;
            for (int count = 0; count < numHouse; count++)
            {
                if ((left < housePic[count].Right) &&
                (right > housePic[count].Left) &&
                (top < housePic[count].Bottom) &&
                (bottom > housePic[count].Top))
                {
                    if (curRoad == 2)
                    {
                        count += 3;
                    }
                    return count;
                }
            }
            return 0;
        }

        //Detecting if the player is currently touching their post office, so their battery can regenerate
        bool goodOfficeCollision(int left, int right, int top, int bottom)
        {
            if ((left < postOffice.Right) &&
                (right > postOffice.Left) &&
                (top < postOffice.Bottom) &&
                (bottom > postOffice.Top))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //Detecting if a bullet has hit an enemy or an enemies bullet
        bool enemyCollision(int left, int right, int top, int bottom)
        {
            PictureBox[] evilPic = {enemyDrone, enemyBulletU, enemyBulletD, enemyBulletL, enemyBulletR, enemyDeliverer, enemyBulletL2, enemyBulletR2 };
            int numEnemy = evilPic.Length;
            for (int count = 0; count < numEnemy; count++)
            {
                if ((left < evilPic[count].Right) &&
                (right > evilPic[count].Left) &&
                (top < evilPic[count].Bottom) &&
                (bottom > evilPic[count].Top))
                {
                    return true;
                }
            }
            return false;
        }

        //Identifies what enemy object has been hit with the bullet
        int bulletCollision(int left, int right, int top, int bottom)
        {
            PictureBox[] evilPic = {enemyBulletU, enemyBulletD, enemyBulletL, enemyBulletR, enemyDrone, enemyBulletL2, enemyBulletR2, enemyDeliverer};
            int numEnemy = evilPic.Length;
            for (int count = 0; count < numEnemy; count++)
            {
                if ((left < evilPic[count].Right) &&
                (right > evilPic[count].Left) &&
                (top < evilPic[count].Bottom) &&
                (bottom > evilPic[count].Top))
                {
                    return count;
                }
            }
            return 10;
        }

        //Detecting if the plyer is currently hovering over a purchaseable upgrade
        bool shopCollisions(int left, int right, int top, int bottom)
        {
            PictureBox[] shopPic = { shopBattery, shopDrone, shopBullets };
            int shopNum = shopPic.Length;
            for (int count = 0; count < shopNum; count++)
            {
                if ((left < shopPic[count].Right) &&
                (right > shopPic[count].Left) &&
                (top < shopPic[count].Bottom) &&
                (bottom > shopPic[count].Top))
                {
                    return true;
                }
            }
            return false;
        }

        //Identifies what upgrade the player is hovering over
        int shopCheck(int left, int right, int top, int bottom)
        {
            PictureBox[] shopPic = { shopBattery, shopDrone, shopBullets };
            int shopNum = shopPic.Length;
            for (int count = 0; count < shopNum; count++)
            {
                if ((left < shopPic[count].Right) &&
                (right > shopPic[count].Left) &&
                (top < shopPic[count].Bottom) &&
                (bottom > shopPic[count].Top))
                {
                    return count;
                }
            }
            return 3;
        }

        //End the current day,
        //deactivates timers and moves objects out of the way and moves in and activated shop related objects,
        //displays a messagebox to the player their results in the day and how they performed
        void endDay()
        {
            //refills battery at different levels depending if the battery upgrade has been purchased
            if (batteryUpgraded == false)
            {
                curBattery = 1000;
            }
            else
            {
                curBattery = 2000;
            }

            //temporarily stops all processes and removes all objects in preparation for the opening of the messagebox
            parcelAttached = false;
            deadDrone = false;
            enemyActivea = false;
            enemyBulletU.Top = enemyBulletU.Top - 1000;
            enemyBulletD.Top = enemyBulletD.Top - 1000;
            enemyBulletL.Left = enemyBulletL.Top - 1000;
            enemyBulletR.Left = enemyBulletR.Top - 1500;
            enemyActiveb = false;
            enemyBulletL2.Left = enemyBulletL2.Top - 1000;
            enemyBulletR2.Left = enemyBulletR2.Top - 1500;
            extraTime = false;
            playerTimer.Enabled = false;
            timeTimer.Enabled = false;
            batteryTimer.Enabled = false;
            deliverTimer.Enabled = false;
            collisionTimer.Enabled = false;
            windTimer.Enabled = false;
            angerTimer.Enabled = false;
            windyTimer.Enabled = false;
            playerMap.Visible = false;
            angryHouse1.Visible = false;
            angryHouse2.Visible = false;
            angryHouse3.Visible = false;
            playerParcel.Visible = false;
            playerDrone.Visible = false;
            enemyDrone.Visible = false;
            enemyDeliverer.Visible = false;

            //Removes houses if on a road with houses, removes post office if on a road with a post office
            if (curRoad == 0)
            {
                house1.Left -= 2000;
                house2.Left -= 2000;
                house3.Left -= 2000;
            }
            else if (curRoad == 1)
            {
                postOffice.Left -= 2000;
            }
            else if (curRoad == 2)
            {
                house1.Left -= 2000;
                house2.Left -= 2000;
                house3.Left -= 2000;
            }
            else if (curRoad == 3)
            {
                postOffice.Left -= 2000;
            }
            
            //Objective label updated to tell the player the day is over
            lblObjective.Text = "DAY COMPLETE";

            //A rank is determined based on the points gathered across the day
            if (dayPts[curDay-1] >= 500)
            {
                dayRank[curDay - 1] = 'S';
            }
            else if (dayPts[curDay - 1] >= 400)
            {
                dayRank[curDay - 1] = 'A';
            }
            else if (dayPts[curDay - 1] >= 300)
            {
                dayRank[curDay - 1] = 'B';
            }
            else if (dayPts[curDay - 1] >= 200)
            {
                dayRank[curDay - 1] = 'C';
            }
            else if (dayPts[curDay - 1] >= 100)
            {
                dayRank[curDay - 1] = 'D';
            }
            else if (dayPts[curDay - 1] >= 50)
            {
                dayRank[curDay - 1] = 'E';
            }
            else if (dayPts[curDay - 1] < 50)
            {
                dayRank[curDay - 1] = 'F';
            }

            //Player movement stops before messagebox opens
            playerLeft = false;
            playerRight = false;
            playerUp = false;
            playerDown = false;

            //Player is put back on the starting road
            curRoad = 1;

            //Resuults for the day are displayed to the player
            MessageBox.Show("DAY " + curDay + " COMPLETE" + "\n\nParcels delivered: " + parcelsDeliveredCorrectly + "/" + totalParcels[curDay - 1] + " * 50pts\nCash earned: " + cashEarned[curDay - 1] + " * 2pts\n\nPoints: " + dayPts[curDay-1] + "\nRank: " + dayRank[curDay-1]);

            //If all days are not complete, open the shop
            if (curDay < 5)
            {
                //make all shop related objects appear
                shop1.Visible = true;
                shop2.Visible = true;
                shop3.Visible = true;
                if (batteryUpgraded == false)
                {
                    shopBattery.Visible = true;
                    shopLbl1.Text = "Battery - 500";
                }
                if (droneUpgraded == false)
                {
                    shopDrone.Visible = true;
                    shopLbl2.Text = "Wind resist - 200";
                }
                if (bulletsUpgraded == false)
                {
                    shopBullets.Visible = true;
                    shopLbl3.Text = "Stronger bullets - 350";
                }
                shopLbl1.Visible = true;
                shopLbl2.Visible = true;
                shopLbl3.Visible = true;

                //places the player at the top of the screen
                playerDrone.Top = shopDrone.Top - 250;
                playerDrone.Left = shopDrone.Left;

                //Re enabled timers to allow for player movement
                playerTimer.Enabled = true;
                collisionTimer.Enabled = true;
                windTimer.Enabled = true;
                playerDrone.Visible = true;

                //The shop is defined as currently open
                inShop = true;

                //Player is notified they are currently in the shop and what controls they have to use
                lblObjective.Text = "SHOP - Press SPACE to buy and ENTER to continue";
            }
            //If all days are complete, end the game
            else
            {
                //Player is notified the game is over
                lblObjective.Text = "GAME OVER";

                //Total rank is calculated based upon the total points collected across the game
                if (totalPts >= 2500)
                {
                    totalRank = 'S';
                }
                else if (totalPts >= 2000)
                {
                    totalRank = 'A';
                }
                else if (totalPts >= 1500)
                {
                    totalRank = 'B';
                }
                else if (totalPts >= 1000)
                {
                    totalRank = 'C';
                }
                else if (totalPts >= 500)
                {
                    totalRank = 'D';
                }
                else if (totalPts >= 250)
                {
                    totalRank = 'E';
                }
                else if (totalPts < 250)
                {
                    totalRank = 'F';
                }

                //Checking if the new score beats the old high score
                if (totalPts > highScore)
                {
                    highScore = totalPts;

                    //Player is told that they have achived a new high score
                    MessageBox.Show("New high score!");
                }

                //Total results for each day and are all calculated to display a final total points and rank, alongside the current high score
                MessageBox.Show("GAME OVER" +
                    "\n\nDay 1:\nParcels Delivered: " + parcelsDeliveredAll[0] + "/" + totalParcels[0] + "\nCash earned: " + cashEarned[0] + "\nPoints:" + dayPts[0] + "\nRank: " + dayRank[0] +
                    "\n\nDay 2:\nParcels Delivered: " + parcelsDeliveredAll[1] + "/" + totalParcels[1] + "\nCash earned: " + cashEarned[1] + "\nPoints:" + dayPts[1] + "\nRank: " + dayRank[1] +
                    "\n\nDay 3:\nParcels Delivered: " + parcelsDeliveredAll[2] + "/" + totalParcels[2] + "\nCash earned: " + cashEarned[2] + "\nPoints:" + dayPts[2] + "\nRank: " + dayRank[2] +
                    "\n\nDay 4:\nParcels Delivered: " + parcelsDeliveredAll[3] + "/" + totalParcels[3] + "\nCash earned: " + cashEarned[3] + "\nPoints:" + dayPts[3] + "\nRank: " + dayRank[3] +
                    "\n\nDay 5:\nParcels Delivered: " + parcelsDeliveredAll[4] + "/" + totalParcels[4] + "\nCash earned: " + cashEarned[4] + "\nPoints:" + dayPts[4] + "\nRank: " + dayRank[4] +
                    "\n\nTotal Points: " + totalPts + "\nOverall Rank: " + totalRank + "\n\nHighscore: " + highScore);

                //The game is restarted and a new house order is decided for each day
                for (int x = 0; x < 5; x++)
                {

                    //Allows for the random selcted houses to not have the same result twice in a day, meaning all deliveries will be different
                    HashSet<int> usedNumbers = new HashSet<int>();
                    
                    for (int y = 0; y < 5; y++)
                    {
                        //Random number that is assigned to what houses have requested delivery for all of the days
                        int randomNumber;

                        do
                        {
                            randomNumber = houseRng.Next(0, 6);
                        }
                        while (usedNumbers.Contains(randomNumber));

                        houseCheck[x, y] = randomNumber;
                        houseName[x, y] = "House " + (randomNumber + 1);
                        usedNumbers.Add(randomNumber);
                    }
                }

                //Objective is updated with the first house the player needs to deliver to
                lblObjective.Text = "OBJECTIVE: Deliver to " + houseName[0, 0];
                
                //All vairables are returned to their original state as they were when the game started before, with the exception of the high score
                inShop = false;
                curDay = 1;
                postOffice.Image = goodOfficePic.Image;
                parcelsDelivered = 0;
                parcelsDeliveredCorrectly = 0;
                curBattery = 1000;
                curRoad = 1;
                timeRemaining = 50;
                playerMap.Image = map1a.Image;
                playerTimer.Enabled = true;
                timeTimer.Enabled = true;
                batteryTimer.Enabled = true;
                deliverTimer.Enabled = true;
                collisionTimer.Enabled = true;
                windTimer.Enabled = true;
                angerTimer.Enabled = true;
                windyTimer.Enabled = true;
                playerParcel.Visible = true;
                playerParcel.Top = parcelArea.Top;
                playerParcel.Left = parcelArea.Left;
                postOffice.Left += 2000;
                playerDrone.Top = shopDrone.Top - 250;
                playerDrone.Left = shopDrone.Left;
                curCash = 0;
                totalPts = 0;
                for (int j = 0; j < 5; j++)
                {
                    dayPts[j] = 0;
                    cashEarned[j] = 0;
                    parcelsDeliveredAll[j] = 0;
                }
                batteryUpgraded = false;
                playerBattery2.Visible = false;
                droneUpgraded = false;
                bulletsUpgraded = false;
                playerDrone.Image = playerDronePic.Image;
                playerDrone.Visible = true;
                playerBulletU.Image = playerBulletU3.Image;
                playerBulletD.Image = playerBulletD3.Image;
                playerBulletL.Image = playerBulletL3.Image;
                playerBulletR.Image = playerBulletR3.Image;

                //Labels are updated to display the orignal values
                lblDay.Text = "DAY: " + curDay;
                lblCash.Text = "CASH: " + curCash;
                lblParcel.Text = "PARCELS: " + parcelsDelivered + "/" + totalParcels[curDay - 1];
                lblTime.Text = "TIME: " + timeRemaining / 60 + ":" + (timeRemaining % 60).ToString("00");
            }
        }

        //Allows the player to access the enemies post office once delivering all parcels
        void startExtraTime()
        {
            //Player can no longer deliver parcels
            deliverTimer.Enabled = false;

            //Extra time has begun
            extraTime = true;

            //Player is told what they can do during the extra time
            lblObjective.Text = "Use your remaining time to attack the opposing post office!";

            //Map is updated to display the new road that is avalible during extra time
            if (curRoad == 0)
            {
                playerMap.Image = map0b.Image;
            }
            else if (curRoad == 1)
            {
                playerMap.Image = map1b.Image;
            }
            else if  (curRoad == 2)
            {
                playerMap.Image = map2b.Image;
            }
        }

        //Re enables all timers and objects and updates information to be accurate to the new day
        void newDay()
        {
            inShop = false;
            curDay++;
            shop1.Visible = false;
            shop2.Visible = false;
            shop3.Visible = false;
            shopBattery.Visible = false;
            shopDrone.Visible = false;
            shopBullets.Visible = false;
            shopLbl1.Visible = false;
            shopLbl2.Visible = false;
            shopLbl3.Visible = false;
            postOffice.Image = goodOfficePic.Image;
            parcelsDelivered = 0;
            parcelsDeliveredCorrectly = 0;
            if (batteryUpgraded == false)
            {
                curBattery = 1000;
            }
            else
            {
                curBattery = 2000;
            }
            curRoad = 1;
            timeRemaining = 50;
            playerMap.Image = map1a.Image;
            playerTimer.Enabled = true;
            timeTimer.Enabled = true;
            batteryTimer.Enabled = true;
            deliverTimer.Enabled = true;
            collisionTimer.Enabled = true;
            windTimer.Enabled = true;
            angerTimer.Enabled = true;
            windyTimer.Enabled = true;
            postOffice.Left += 2000;
            playerDrone.Top = shopDrone.Top - 250;
            playerDrone.Left = shopDrone.Left;
            playerParcel.Visible = true;
            playerParcel.Left = parcelArea.Left;
            playerParcel.Top = parcelArea.Top;
            lblObjective.Text = "OBJECTIVE: Deliver to " + houseName[curDay - 1, parcelsDelivered];
            lblDay.Text = "DAY: " + curDay;
            lblCash.Text = "CASH: " + curCash;
            lblParcel.Text = "PARCELS: " + parcelsDelivered + "/" + totalParcels[curDay - 1];
            lblTime.Text = "TIME: " + timeRemaining / 60 + ":" + (timeRemaining % 60).ToString("00");
        }

        //Responsible for moving the player when under control
        private void playerTimer_Tick(object sender, EventArgs e)
        {
            //Moving the player up
            if ((playerUp == true) && (!droneCollisions(playerDrone.Left, playerDrone.Right, playerDrone.Top-6, playerDrone.Bottom)))
            {
                playerDrone.Top = playerDrone.Top - 5;

                //Moving the parcel up when attached
                if ((parcelAttached == true) && (!droneCollisionsExcHouse(playerParcel.Left, playerParcel.Right, playerParcel.Top-6, playerParcel.Bottom)))
                {
                    playerParcel.Top = playerParcel.Top - 5;
                }
            }
            //Moving the player down
            if ((playerDown == true) && (!droneCollisions(playerDrone.Left, playerDrone.Right, playerDrone.Top, playerDrone.Bottom+6)))
            {
                playerDrone.Top = playerDrone.Top + 5;

                //Moving the parcel down when attached
                if ((parcelAttached == true) && (!droneCollisionsExcHouse(playerParcel.Left, playerParcel.Right, playerParcel.Top, playerParcel.Bottom+6)))
                {
                    playerParcel.Top = playerParcel.Top + 5;
                }
            }
            //moving the player left
            if ((playerLeft == true) && (!droneCollisions(playerDrone.Left-6, playerDrone.Right, playerDrone.Top, playerDrone.Bottom)) && !(playerDrone.Left < this.Width - 1000))
            {
                playerDrone.Left = playerDrone.Left - 5;

                //Moving the parcel left when attached
                if ((parcelAttached == true) && (!droneCollisionsExcHouse(playerParcel.Left-6, playerParcel.Right, playerParcel.Top, playerParcel.Bottom)))
                {
                    playerParcel.Left = playerParcel.Left - 5;
                }
            }
            //Moving the player right
            if ((playerRight == true) && (!droneCollisions(playerDrone.Left, playerDrone.Right+6, playerDrone.Top, playerDrone.Bottom)) && !(playerDrone.Right > this.Width - 30))
            {
                playerDrone.Left = playerDrone.Left + 5;

                //Moving the parcel right when attached
                if ((parcelAttached == true) && (!droneCollisionsExcHouse(playerParcel.Left, playerParcel.Right+6, playerParcel.Top, playerParcel.Bottom)))
                {
                    playerParcel.Left = playerParcel.Left + 5;
                }
            }
        }

        //Allows for player control when a control button is pressed
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //Player control is only avalible while the player is alive
            if (deadDrone == false)
            {
                //Moving the player up
                if (e.KeyCode == Keys.W)
                {
                    playerUp = true;
                }
                //Moving the player down
                if (e.KeyCode == Keys.S)
                {
                    playerDown = true;
                }
                //moving the player left
                if (e.KeyCode == Keys.A)
                {
                    playerLeft = true;
                }
                //Moving the player right
                if (e.KeyCode == Keys.D)
                {
                    playerRight = true;
                }

                //Dropping and picking up parcels
                if (e.KeyCode == Keys.Space && parcelCollisions(playerDrone.Left, playerDrone.Right, playerDrone.Top, playerDrone.Bottom))
                {
                    if (parcelAttached == false)
                    {
                        parcelAttached = true;
                    }
                    else
                    {
                        parcelAttached = false;
                        parcelFall = true;
                    }
                    curBattery = curBattery - 25;
                }

                //Scroll though the map while not inside the shop
                if (inShop == false)
                {
                    if (parcelFall == false)
                    {
                        //Move left on the map
                        if (e.KeyCode == Keys.Q)
                        {
                            if (curRoad == 0 && extraTime == true)
                            {
                                curRoad = 3;
                                house1.Left -= 2000;
                                house2.Left -= 2000;
                                house3.Left -= 2000;
                                postOffice.Image = badOfficePic.Image;
                                postOffice.Left += 2000;
                                playerMap.Image = map3.Image;
                            }
                            else if (curRoad == 1)
                            {
                                house1.Left += 2000;
                                house2.Left += 2000;
                                house3.Left += 2000;
                                if (droneCollisions(playerDrone.Left, playerDrone.Right, playerDrone.Top, playerDrone.Bottom))
                                {
                                    house1.Left -= 2000;
                                    house2.Left -= 2000;
                                    house3.Left -= 2000;
                                }
                                else
                                {
                                    curRoad--;
                                    postOffice.Left -= 2000;
                                    if (playerParcel.Visible == true && parcelAttached == false && droneCollisionsExcHouse(playerParcel.Left, playerParcel.Right, playerParcel.Top, playerParcel.Bottom + 6))
                                    {
                                        playerParcel.Visible = false;
                                        playerParcel.Left -= 1000;
                                    }
                                    house1.Image = housePic1.Image;
                                    house2.Image = housePic2.Image;
                                    house3.Image = housePic3.Image;
                                    house1.Visible = true;
                                    house2.Visible = true;
                                    house3.Visible = true;
                                    if (extraTime == false)
                                    {
                                        playerMap.Image = map0a.Image;
                                    }
                                    else
                                    {
                                        playerMap.Image = map0b.Image;
                                    }
                                }
                            }
                            else if (curRoad == 2)
                            {
                                curRoad--;
                                house1.Left -= 2000;
                                house2.Left -= 2000;
                                house3.Left -= 2000;
                                postOffice.Left += 2000;
                                if (playerParcel.Visible == false && parcelsDelivered < 5)
                                {
                                    playerParcel.Left = parcelArea.Left;
                                    playerParcel.Top = parcelArea.Top;
                                    playerParcel.Visible = true;
                                }
                                if (extraTime == false)
                                {
                                    playerMap.Image = map1a.Image;
                                }
                                else
                                {
                                    playerMap.Image = map1b.Image;
                                }
                            }
                            else if (curRoad == 3)
                            {
                                house1.Left += 2000;
                                house2.Left += 2000;
                                house3.Left += 2000;
                                if (droneCollisions(playerDrone.Left, playerDrone.Right, playerDrone.Top, playerDrone.Bottom))
                                {
                                    house1.Left -= 2000;
                                    house2.Left -= 2000;
                                    house3.Left -= 2000;
                                }
                                else
                                {
                                    curRoad = 0;
                                    house1.Image = housePic1.Image;
                                    house2.Image = housePic2.Image;
                                    house3.Image = housePic3.Image;
                                    postOffice.Left -= 2000;
                                    postOffice.Image = goodOfficePic.Image;
                                    playerMap.Image = map0b.Image;
                                }
                            }
                        }

                        //Move right on the map
                        if (e.KeyCode == Keys.E)
                        {
                            if (curRoad == 0)
                            {
                                curRoad++;
                                house1.Left -= 2000;
                                house2.Left -= 2000;
                                house3.Left -= 2000;
                                postOffice.Left += 2000;
                                if (playerParcel.Visible == false && parcelsDelivered < 5)
                                {
                                    playerParcel.Left = parcelArea.Left;
                                    playerParcel.Top = parcelArea.Top;
                                    playerParcel.Visible = true;
                                }
                                if (extraTime == false)
                                {
                                    playerMap.Image = map1a.Image;
                                }
                                else
                                {
                                    playerMap.Image = map1b.Image;
                                }
                            }
                            else if (curRoad == 1)
                            {
                                house1.Left += 2000;
                                house2.Left += 2000;
                                house3.Left += 2000;
                                if (droneCollisions(playerDrone.Left, playerDrone.Right, playerDrone.Top, playerDrone.Bottom))
                                {
                                    house1.Left -= 2000;
                                    house2.Left -= 2000;
                                    house3.Left -= 2000;
                                }
                                else
                                {
                                    curRoad++;
                                    postOffice.Left -= 2000;
                                    if (playerParcel.Visible == true && parcelAttached == false && droneCollisionsExcHouse(playerParcel.Left, playerParcel.Right, playerParcel.Top, playerParcel.Bottom + 6))
                                    {
                                        playerParcel.Visible = false;
                                        playerParcel.Left -= 1000;
                                    }
                                    house1.Image = housePic4.Image;
                                    house2.Image = housePic5.Image;
                                    house3.Image = housePic6.Image;
                                    house1.Visible = true;
                                    house2.Visible = true;
                                    house3.Visible = true;
                                    if (extraTime == false)
                                    {
                                        playerMap.Image = map2a.Image;
                                    }
                                    else
                                    {
                                        playerMap.Image = map2b.Image;
                                    }
                                }
                            }
                            else if (curRoad == 2 && extraTime == true)
                            {
                                curRoad = 3;
                                house1.Left -= 2000;
                                house2.Left -= 2000;
                                house3.Left -= 2000;
                                postOffice.Image = badOfficePic.Image;
                                postOffice.Left += 2000;
                                playerMap.Image = map3.Image;
                            }
                            else if (curRoad == 3)
                            {
                                house1.Left += 2000;
                                house2.Left += 2000;
                                house3.Left += 2000;
                                if (droneCollisions(playerDrone.Left, playerDrone.Right, playerDrone.Top, playerDrone.Bottom))
                                {
                                    house1.Left -= 2000;
                                    house2.Left -= 2000;
                                    house3.Left -= 2000;
                                }
                                else
                                {
                                    curRoad = 2;
                                    postOffice.Left -= 2000;
                                    house1.Image = housePic4.Image;
                                    house2.Image = housePic5.Image;
                                    house3.Image = housePic6.Image;
                                    postOffice.Image = goodOfficePic.Image;
                                    playerMap.Image = map2b.Image;
                                }
                            }
                        }
                    }
                    //Open the map
                    if (e.KeyCode == Keys.M)
                    {
                        playerMap.Visible = true;
                    }
                }
                else
                {
                    //Start a new day and exit the shop
                    if (e.KeyCode == Keys.Enter)
                    {
                        newDay();
                    }

                    //Purchasing upgrades in the shop
                    if (e.KeyCode == Keys.Space && shopCollisions(playerDrone.Left, playerDrone.Right, playerDrone.Top, playerDrone.Bottom))
                    {
                        if (shopCheck(playerDrone.Left, playerDrone.Right, playerDrone.Top, playerDrone.Bottom) == 0 && batteryUpgraded == false && curCash >= 500)
                        {
                            curCash -= 500;
                            shopBattery.Visible = false;
                            lblCash.Text = "CASH: " + curCash;
                            shopLbl1.Text = "SOLD";
                            batteryUpgraded = true;
                            curBattery = 2000;
                            playerBattery2.Visible = true;
                        }
                        if (shopCheck(playerDrone.Left, playerDrone.Right, playerDrone.Top, playerDrone.Bottom) == 1 && droneUpgraded == false && curCash >= 200)
                        {
                            curCash -= 200;
                            shopDrone.Visible = false;
                            lblCash.Text = "CASH: " + curCash;
                            shopLbl2.Text = "SOLD"; 
                            playerDrone.Image = playerDronePic2.Image;
                            droneUpgraded = true;
                        }
                        if (shopCheck(playerDrone.Left, playerDrone.Right, playerDrone.Top, playerDrone.Bottom) == 2 && bulletsUpgraded == false && curCash >= 350)
                        {
                            curCash -= 350;
                            shopBullets.Visible = false;
                            lblCash.Text = "CASH: " + curCash;
                            shopLbl3.Text = "SOLD";
                            bulletsUpgraded = true;
                            playerBulletU.Image = playerBulletU2.Image;
                            playerBulletD.Image = playerBulletD2.Image;
                            playerBulletL.Image = playerBulletL2.Image;
                            playerBulletR.Image = playerBulletR2.Image;
                        }
                    }
                }
            }
        }

        //Allows for player control to stop when a control button is released
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (deadDrone == false)
            {
                //Stop moving up
                if (e.KeyCode == Keys.W)
                {
                    playerUp = false;
                }
                //Stop moving down
                if (e.KeyCode == Keys.S)
                {
                    playerDown = false;
                }
                //Stop moving left
                if (e.KeyCode == Keys.A)
                {
                    playerLeft = false;
                }
                //Stop moving right
                if (e.KeyCode == Keys.D)
                {
                    playerRight = false;
                }

                //Shoot up
                if (e.KeyCode == Keys.Up)
                {

                    if (playerShootUp == false && playerShoot == false)
                    {
                        playerBulletU.Left = playerDrone.Left;
                        playerBulletU.Top = playerDrone.Top;
                        playerShoot = true;
                        playerShootUp = true;
                        curBattery = curBattery - 10;
                    }
                }

                //Shoot down
                if (e.KeyCode == Keys.Down)
                {

                    if (playerShootDown == false && playerShoot == false)
                    {
                        playerBulletD.Left = playerDrone.Left;
                        playerBulletD.Top = playerDrone.Top;
                        playerShoot = true;
                        playerShootDown = true;
                        curBattery = curBattery - 10;
                    }
                }

                //Shoot left
                if (e.KeyCode == Keys.Left)
                {

                    if (playerShootLeft == false && playerShoot == false)
                    {
                        playerBulletL.Left = playerDrone.Left;
                        playerBulletL.Top = playerDrone.Top;
                        playerShoot = true;
                        playerShootLeft = true;
                        curBattery = curBattery - 10;
                    }
                }

                //Shoot right
                if (e.KeyCode == Keys.Right)
                {

                    if (playerShootRight == false && playerShoot == false)
                    {
                        playerBulletR.Left = playerDrone.Left;
                        playerBulletR.Top = playerDrone.Top;
                        playerShoot = true;
                        playerShootRight = true;
                        curBattery = curBattery - 10;
                    }
                }

                //Closes map
                if (e.KeyCode == Keys.M)
                {
                    playerMap.Visible = false;
                }
            }
        }

        //Responsible for making the parcel, the player, and enemies fall 
        private void fallTimer_Tick(object sender, EventArgs e)
        {
            //Parcel falling physics
            if ((parcelFall == true) && (!droneCollisionsExcHouse(playerParcel.Left, playerParcel.Right, playerParcel.Top, playerParcel.Bottom+12)))
            {
                playerParcel.Top = playerParcel.Top + fallSpeed;
                fallCount++;

                if (fallCount == 10)
                {
                    fallSpeed = fallSpeed * 2;
                }
                if (fallCount > 10)
                {
                    fallCount = 1;
                }
            }

            //Player falling physics
            if (droneCollisionsExcHouse(playerParcel.Left, playerParcel.Right, playerParcel.Top, playerParcel.Bottom + 6))
            {
                parcelFall = false;
                fallSpeed = 1;
                fallCount = 1;
            }

            if (curBattery == 0)
            {
                playerDrone.Top = playerDrone.Top + playerFallSpeed;
                playerFallCount++;

                if (playerFallCount == 10)
                {
                    playerFallSpeed = playerFallSpeed * 2;
                }
                if (playerFallCount > 10)
                {
                    playerFallCount = 1;
                }
            }

            //Enemy drone falling physics
            if (enemyActivea == false)
            {
                enemyDrone.Top = enemyDrone.Top + enemyFallSpeed;
                enemyFallCount++;

                if (enemyFallCount == 10)
                {
                    enemyFallSpeed = enemyFallSpeed * 2;
                }
                if (enemyFallCount > 10)
                {
                    enemyFallCount = 1;
                }
            }

            //Deliverer drone falling physics
            if (enemyActiveb == false)
            {
                enemyDeliverer.Top = enemyDeliverer.Top + enemyFallSpeedB;
                enemyFallCountB++;

                if (enemyFallCountB == 10)
                {
                    enemyFallSpeedB = enemyFallSpeedB * 2;
                }
                if (enemyFallCountB > 10)
                {
                    enemyFallCountB = 1;
                }
            }
        }

        //Responsible for counting down and updating lblTime with the current time, also responsible for the players battery drain
        private void timeTimer_Tick(object sender, EventArgs e)
        {
            timeRemaining--;
            lblTime.Text = "TIME: " + timeRemaining / 60 + ":" + (timeRemaining % 60).ToString("00");

            //When the time runs out, the day is over
            if (timeRemaining == 0)
            {
                endDay();
            }    

            if (deadDrone == false)
            {
                if (batteryUpgraded == false)
                {
                    //Gain battery when at the post office, lose battery when not
                    if (goodOfficeCollision(playerDrone.Left, playerDrone.Right, playerDrone.Top, playerDrone.Bottom) && curBattery < 1000 && curRoad == 1)
                    {
                        curBattery = curBattery + 20;
                    }
                    else
                    {
                        curBattery = curBattery - 4;
                    }
                }
                else
                {
                    //Gain battery when at the post office, lose battery when not
                    if (goodOfficeCollision(playerDrone.Left, playerDrone.Right, playerDrone.Top, playerDrone.Bottom) && curBattery < 2000 && curRoad == 1)
                    {
                        curBattery = curBattery + 20;
                    }
                    else
                    {
                        curBattery = curBattery - 4;
                    }
                }
            }
        }

        //Responsible for updating the battery icons to represent the current battery staus, ends the day if the battery reaches 0
        private void batteryTimer_Tick(object sender, EventArgs e)
        {
            if (deadDrone == false)
            {
                if (curBattery > 1650)
                {
                    playerBattery1.Image = batteryPic1.Image;
                    playerBattery2.Image = batteryPic1.Image;
                }
                else if ((curBattery < 1650) && !(curBattery < 1300))
                {
                    playerBattery1.Image = batteryPic1.Image;
                    playerBattery2.Image = batteryPic2.Image;
                }
                else if ((curBattery < 1300) && !(curBattery < 1000))
                {
                    playerBattery1.Image = batteryPic1.Image;
                    playerBattery2.Image = batteryPic3.Image;
                }
                else if ((curBattery < 1000) && !(curBattery < 650))
                {
                    playerBattery1.Image = batteryPic1.Image;
                    playerBattery2.Image = batteryPic4.Image;
                }
                else if ((curBattery < 650) && !(curBattery < 300))
                {
                    playerBattery1.Image = batteryPic2.Image;
                    playerBattery2.Image = batteryPic4.Image;
                }
                else if (curBattery < 300 && !(curBattery <= 0))
                {
                    playerBattery1.Image = batteryPic3.Image;
                    playerBattery2.Image = batteryPic4.Image;
                }
                else if (curBattery <= 0)
                {
                    playerBattery1.Image = batteryPic4.Image;
                    playerBattery2.Image = batteryPic4.Image;
                    deadDrone = true;

                    //Day ends if the players battery reaches 0
                    endDay();
                }
            }
        }

        //Responsible for checking if the parcel has been delivered to the correct household updating lblParcel, lblCash, and lblObjective with current variables
        private void deliverTimer_Tick(object sender, EventArgs e)
        {
            if ((parcelAttached == false) && houseCollisions(playerParcel.Left, playerParcel.Right, playerParcel.Top, playerParcel.Bottom))
            {
                //the correct house is selected
                if (houseCheck[curDay - 1, parcelsDelivered] == houseNumCheck(playerParcel.Left, playerParcel.Right, playerParcel.Top, playerParcel.Bottom))
                {
                    parcelFall = false;
                    fallSpeed = 1;
                    fallCount = 1;
                    parcelsDelivered++;

                    //Delivered damaged parcels yield less profit
                    if (parcelDamaged == true)
                    {
                        curCash += 25;
                        cashEarned[curDay - 1] += 25;
                        parcelsDeliveredCorrectly += 0.5;
                        parcelsDeliveredAll[curDay - 1] += 0.5;
                        dayPts[curDay - 1] += 50;
                        totalPts += 50;
                    }
                    //Broken parcels will not give any money
                    else if (parcelBroken == false)
                    {
                        curCash += 50;
                        cashEarned[curDay - 1] += 50;
                        parcelsDeliveredCorrectly++;
                        parcelsDeliveredAll[curDay - 1]++;
                        dayPts[curDay - 1] += 100;
                        totalPts += 100;
                    }
                    //Parcel disapears off the screemn
                    playerParcel.Left -= 1000;
                    playerParcel.Visible = false;
                    parcelBroken = false;
                    parcelDamaged = false;

                    //Cash label is updated to represent the new value
                    lblCash.Text = "CASH: " + curCash;
                }
                //The wrong house is selected
                else
                {
                    angryHouse = true;
                    curHouse = houseNumCheck(playerParcel.Left, playerParcel.Right, playerParcel.Top, playerParcel.Bottom);
                    if (curRoad == 2)
                    {
                        curHouse -= 3;
                    }
                    parcelFall = false;
                    fallSpeed = 1;
                    fallCount = 1;

                    playerParcel.Left -= 1000;
                    playerParcel.Visible = false;
                    parcelBroken = false;
                    parcelDamaged = false;
                    parcelsDelivered++;
                }

                //The amount of parcels delivered is updated
                lblParcel.Text = "PARCELS: " + parcelsDelivered + "/" + totalParcels[curDay - 1];

                //Extra time begins once all parcels have been delivered for the day
                if (totalParcels[curDay - 1] == parcelsDelivered)
                {
                    startExtraTime();
                }
                //If there are more parcels remaining the player is informed what house is next on the list
                else
                {
                    lblObjective.Text = "OBJECTIVE: Deliver to " + houseName[curDay - 1, parcelsDelivered];
                }
            }
            //Parcel lands on the ground instead of a house
            else if ((parcelAttached == false) && groundCollisions(playerParcel.Left, playerParcel.Right, playerParcel.Top, playerParcel.Bottom + 12) && (playerParcel.Left != parcelArea.Left && playerParcel.Top != parcelArea.Top))
            {
                parcelFall = false;
                fallSpeed = 1;
                fallCount = 1;
                //If the player is already at the post office, the next parcel shows up where it should be if there are more remaining
                if (curRoad == 1 && parcelsDelivered < 4)
                {
                    playerParcel.Left = parcelArea.Left;
                    playerParcel.Top = parcelArea.Top;
                }
                //Parcel is destroyed(dissapears)
                else
                {
                    playerParcel.Left -= 1000;
                    playerParcel.Visible = false;
                }
                parcelBroken = false;
                parcelDamaged = false;
                parcelsDelivered++;
                lblParcel.Text = "PARCELS: " + parcelsDelivered + "/" + totalParcels[curDay - 1];

                //Extra time begins once all parcels have been delivered for the day
                if (totalParcels[curDay - 1] == parcelsDelivered)
                {
                    startExtraTime();
                }
                //If there are more parcels remaining the player is informed what house is next on the list
                else
                {
                    lblObjective.Text = "OBJECTIVE: Deliver to " + houseName[curDay - 1, parcelsDelivered];
                }
            }
        }

        //Sets the houses that need deliveries for the game and the intensity of the wind for each day
        private void Form1_Load(object sender, EventArgs e)
        {
            for (int x = 0; x < 5; x++)
            {

                //Allows for the random selcted houses to not have the same result twice in a day, meaning all deliveries will be different
                HashSet<int> usedNumbers = new HashSet<int>();

                for (int y = 0; y < 5; y++)
                {
                    //Random number that is assigned to what houses have requested delivery for all of the days
                    int randomNumber;

                    do
                    {
                        randomNumber = houseRng.Next(0, 6);
                    }
                    while (usedNumbers.Contains(randomNumber));

                    houseCheck[x, y] = randomNumber;
                    houseName[x, y] = "House " + (randomNumber + 1);
                    usedNumbers.Add(randomNumber);
                }
            }

            lblObjective.Text = "OBJECTIVE: Deliver to " + houseName[0, 0];

            //set values of what wind direction and intensity will be for each day
            //1 = weak left
            //2 = weak right
            //3 = medium left
            //4 = medium right
            //5 = strong left
            //6 = strong right

            windDay[0] = 1;
            windDay[1] = 2;
            windDay[2] = 3;
            windDay[3] = 4;
            windDay[4] = 5;

            //houses are temporarily removed as the game starts at the post office
            house1.Left -= 2000;
            house2.Left -= 2000;
            house3.Left -= 2000;
        }

        //Responsible for the shooting of both the player and enemies, finding what the bullet has hit using collision
        private void shootTimer_Tick(object sender, EventArgs e)
        {
            //The player can only shoot 1 bullet at a time
            if (playerShoot == true)
            {
                //Player shooting up
                if (playerShootUp == true)
                {
                    playerBulletU.Top = playerBulletU.Top - 10;
                    //Bullet dissapearing allowing for another shot
                    if (playerBulletU.Bottom < 0)
                    {
                        playerShoot = false;
                        playerShootUp = false;
                    }
                    //Detecting what object the bullet has collided with
                    else if (enemyCollision(playerBulletU.Left, playerBulletU.Right, playerBulletU.Top - 6, playerBulletU.Bottom))
                    {
                        //enemyBulletU
                        if (bulletCollision(playerBulletU.Left, playerBulletU.Right, playerBulletU.Top - 6, playerBulletU.Bottom) == 0)
                        {
                            playerBulletU.Top = playerBulletU.Top - 1000;
                            enemyBulletU.Top = enemyBulletU.Top - 1000;
                        }
                        //enemyBulletD
                        else if (bulletCollision(playerBulletU.Left, playerBulletU.Right, playerBulletU.Top - 6, playerBulletU.Bottom) == 1)
                        {
                            playerBulletU.Top = playerBulletU.Top - 1000;
                            enemyBulletD.Top = enemyBulletD.Top + 1000;
                        }
                        //enemyBulletL
                        else if (bulletCollision(playerBulletU.Left, playerBulletU.Right, playerBulletU.Top - 6, playerBulletU.Bottom) == 2)
                        {
                            playerBulletU.Top = playerBulletU.Top - 1000;
                            enemyBulletL.Left = enemyBulletL.Left - 1000;
                        }
                        //enemyBulletR
                        else if (bulletCollision(playerBulletU.Left, playerBulletU.Right, playerBulletU.Top - 6, playerBulletU.Bottom) == 3)
                        {
                            playerBulletU.Top = playerBulletU.Top - 1000;
                            enemyBulletR.Left = enemyBulletR.Left + 1000;
                        }
                        //enemyDrone
                        else if (bulletCollision(playerBulletU.Left, playerBulletU.Right, playerBulletU.Top - 6, playerBulletU.Bottom) == 4)
                        {
                            playerBulletU.Top = playerBulletU.Top - 1000;
                            //Destroyed in two shots without an upgrade
                            if (bulletsUpgraded == false)
                            {
                                if (enemyDamaged == true)
                                {
                                    if (batteryUpgraded == false)
                                    {
                                        if (curBattery < 1000)
                                        {
                                            curBattery += 50;
                                        }
                                    }
                                    else
                                    {
                                        if (curBattery < 2000)
                                        {
                                            curBattery += 50;
                                        }
                                    }
                                    enemyActivea = false;
                                    enemyBulletU.Top = enemyBulletU.Top - 1000;
                                    enemyBulletD.Top = enemyBulletD.Top - 1000;
                                    enemyBulletL.Left = enemyBulletL.Top - 1000;
                                    enemyBulletR.Left = enemyBulletR.Top - 1500;
                                }
                                else
                                {
                                    enemyDamaged = true;
                                }
                            }
                            //Destroyed in one shot with an upgrade
                            else
                            {
                                if (batteryUpgraded == false)
                                {
                                    if (curBattery < 1000)
                                    {
                                        curBattery += 50;
                                    }
                                }
                                else
                                {
                                    if (curBattery < 2000)
                                    {
                                        curBattery += 50;
                                    }
                                }
                                enemyActivea = false;
                                enemyBulletU.Top = enemyBulletU.Top - 1000;
                                enemyBulletD.Top = enemyBulletD.Top - 1000;
                                enemyBulletL.Left = enemyBulletL.Top - 1000;
                                enemyBulletR.Left = enemyBulletR.Top - 1500;
                            }
                        }
                        //enemyBulletL2
                        else if (bulletCollision(playerBulletU.Left, playerBulletU.Right, playerBulletU.Top - 6, playerBulletU.Bottom) == 5)
                        {
                            playerBulletU.Top = playerBulletU.Top - 1000;
                            enemyBulletR2.Left = enemyBulletR2.Left + 1000;
                        }
                        //enemyBulletR2
                        else if (bulletCollision(playerBulletU.Left, playerBulletU.Right, playerBulletU.Top - 6, playerBulletU.Bottom) == 6)
                        {
                            playerBulletU.Top = playerBulletU.Top - 1000;
                            enemyBulletL2.Left = enemyBulletL2.Left + 1000;
                        }
                        //enemyDeliverer
                        else if (bulletCollision(playerBulletU.Left, playerBulletU.Right, playerBulletU.Top - 6, playerBulletU.Bottom) == 7)
                        {
                            playerBulletU.Top = playerBulletU.Top - 1000;
                            //Destroyed in two shots without an upgrade
                            if (bulletsUpgraded == false)
                            {
                                if (enemyDamagedB == true)
                                {
                                    if (batteryUpgraded == false)
                                    {
                                        if (curBattery < 1000)
                                        {
                                            curBattery += 50;
                                        }
                                    }
                                    else
                                    {
                                        if (curBattery < 2000)
                                        {
                                            curBattery += 50;
                                        }
                                    }
                                    enemyActiveb = false;
                                    enemyBulletL2.Left = enemyBulletL2.Top - 1000;
                                    enemyBulletR2.Left = enemyBulletR2.Top - 1500;
                                    curCash += 25;
                                    dayPts[curDay - 1] += 50;
                                    cashEarned[curDay - 1] += 25;
                                    lblCash.Text = "CASH: " + curCash;
                                }
                                else
                                {
                                    enemyDamagedB = true;
                                }
                            }
                            //Destroyed in one shot with an upgrade
                            else
                            {
                                if (batteryUpgraded == false)
                                {
                                    if (curBattery < 1000)
                                    {
                                        curBattery += 50;
                                    }
                                }
                                else
                                {
                                    if (curBattery < 2000)
                                    {
                                        curBattery += 50;
                                    }
                                }
                                enemyActiveb = false;
                                enemyBulletL2.Left = enemyBulletL2.Top - 1000;
                                enemyBulletR2.Left = enemyBulletR2.Top - 1500;
                                curCash += 25;
                                dayPts[curDay - 1] += 50;
                                cashEarned[curDay - 1] += 25;
                                lblCash.Text = "CASH: " + curCash;
                            }
                        }
                    }
                }
                //Player shooting down
                if (playerShootDown == true)
                {
                    playerBulletD.Top = playerBulletD.Top + 10;

                    //Bullet dissapearing allowing for another shot
                    if (playerBulletD.Top > 1000)
                    {
                        playerShoot = false;
                        playerShootDown = false;
                    }

                    //Detecting what object the bullet has collided with
                    else if (enemyCollision(playerBulletD.Left, playerBulletD.Right, playerBulletD.Top, playerBulletD.Bottom + 6))
                    {
                        //enemyBulletU
                        if (bulletCollision(playerBulletD.Left, playerBulletD.Right, playerBulletD.Top, playerBulletD.Bottom + 6) == 0)
                        {
                            playerBulletD.Top = playerBulletD.Top + 1000;
                            enemyBulletU.Top = enemyBulletU.Top - 1000;
                        }
                        //enemyBulletD
                        else if (bulletCollision(playerBulletD.Left, playerBulletD.Right, playerBulletD.Top, playerBulletD.Bottom + 6) == 1)
                        {
                            playerBulletD.Top = playerBulletD.Top + 1000;
                            enemyBulletD.Top = enemyBulletD.Top + 1000;
                        }
                        //enemyBulletL
                        else if (bulletCollision(playerBulletD.Left, playerBulletD.Right, playerBulletD.Top, playerBulletD.Bottom + 6) == 2)
                        {
                            playerBulletD.Top = playerBulletD.Top + 1000;
                            enemyBulletL.Left = enemyBulletL.Left - 1000;
                        }
                        //enemybulletR
                        else if (bulletCollision(playerBulletD.Left, playerBulletD.Right, playerBulletD.Top, playerBulletD.Bottom + 6) == 3)
                        {
                            playerBulletD.Top = playerBulletD.Top + 1000;
                            enemyBulletR.Left = enemyBulletR.Left + 1000;
                        }
                        //enemyDrone
                        else if (bulletCollision(playerBulletD.Left, playerBulletD.Right, playerBulletD.Top, playerBulletD.Bottom + 6) == 4)
                        {
                            playerBulletD.Top = playerBulletD.Top + 1000;
                            //Destroyed in two shots without an upgrade
                            if (bulletsUpgraded == false)
                            {
                                if (enemyDamaged == true)
                                {
                                    if (batteryUpgraded == false)
                                    {
                                        if (curBattery < 1000)
                                        {
                                            curBattery += 50;
                                        }
                                    }
                                    else
                                    {
                                        if (curBattery < 2000)
                                        {
                                            curBattery += 50;
                                        }
                                    }
                                    enemyActivea = false;
                                    enemyBulletU.Top = enemyBulletU.Top - 1000;
                                    enemyBulletD.Top = enemyBulletD.Top - 1000;
                                    enemyBulletL.Left = enemyBulletL.Top - 1000;
                                    enemyBulletR.Left = enemyBulletR.Top - 1500;
                                }
                                else
                                {
                                    enemyDamaged = true;
                                }
                            }
                            //Destroyed in one shot with an upgrade
                            else
                            {
                                if (batteryUpgraded == false)
                                {
                                    if (curBattery < 1000)
                                    {
                                        curBattery += 50;
                                    }
                                }
                                else
                                {
                                    if (curBattery < 2000)
                                    {
                                        curBattery += 50;
                                    }
                                }
                                enemyActivea = false;
                                enemyBulletU.Top = enemyBulletU.Top - 1000;
                                enemyBulletD.Top = enemyBulletD.Top - 1000;
                                enemyBulletL.Left = enemyBulletL.Top - 1000;
                                enemyBulletR.Left = enemyBulletR.Top - 1500;
                            }
                        }
                        //enemyBulletL2
                        else if (bulletCollision(playerBulletD.Left, playerBulletD.Right, playerBulletD.Top, playerBulletD.Bottom + 6) == 5)
                        {
                            playerBulletD.Top = playerBulletD.Top + 1000;
                            enemyBulletR2.Left = enemyBulletR2.Left + 1000;
                        }
                        //enemyBulletR2
                        else if (bulletCollision(playerBulletD.Left, playerBulletD.Right, playerBulletD.Top, playerBulletD.Bottom + 6) == 6)
                        {
                            playerBulletD.Top = playerBulletD.Top + 1000;
                            enemyBulletL2.Left = enemyBulletL2.Left + 1000;
                        }
                        //enemyDeliverer
                        else if (bulletCollision(playerBulletD.Left, playerBulletD.Right, playerBulletD.Top, playerBulletD.Bottom + 6) == 7)
                        {
                            playerBulletD.Top = playerBulletD.Top + 1000;
                            //Destroyed in two shots without an upgrade
                            if (bulletsUpgraded == false)
                            {
                                if (enemyDamagedB == true)
                                {
                                    if (batteryUpgraded == false)
                                    {
                                        if (curBattery < 1000)
                                        {
                                            curBattery += 50;
                                        }
                                    }
                                    else
                                    {
                                        if (curBattery < 2000)
                                        {
                                            curBattery += 50;
                                        }
                                    }
                                    enemyActiveb = false;
                                    enemyBulletL2.Left = enemyBulletL2.Top - 1000;
                                    enemyBulletR2.Left = enemyBulletR2.Top - 1500;
                                    curCash += 25;
                                    dayPts[curDay - 1] += 50;
                                    cashEarned[curDay - 1] += 25;
                                    lblCash.Text = "CASH: " + curCash;
                                }
                                else
                                {
                                    enemyDamagedB = true;
                                }
                            }
                            //Destroyed in one shot with an upgrade
                            else
                            {
                                if (batteryUpgraded == false)
                                    {
                                        if (curBattery < 1000)
                                        {
                                            curBattery += 50;
                                        }
                                    }
                                    else
                                    {
                                        if (curBattery < 2000)
                                        {
                                            curBattery += 50;
                                        }
                                    }
                                enemyActiveb = false;
                                enemyBulletL2.Left = enemyBulletL2.Top - 1000;
                                enemyBulletR2.Left = enemyBulletR2.Top - 1500;
                                curCash += 25;
                                dayPts[curDay - 1] += 50;
                                cashEarned[curDay - 1] += 25;
                                lblCash.Text = "CASH: " + curCash;
                            }
                        }
                    }
                }
                //Player shooting left
                if (playerShootLeft == true)
                {
                    playerBulletL.Left = playerBulletL.Left - 10;

                    //Bullet dissapearing allowing for another shot
                    if (playerBulletL.Right < 0)
                    {
                        playerShoot = false;
                        playerShootLeft = false;
                    }
                    //Detecting what object the bullet has collided with
                    else if (enemyCollision(playerBulletL.Left - 6, playerBulletL.Right, playerBulletL.Top, playerBulletL.Bottom))
                    {
                        //enemyBulletU
                        if (bulletCollision(playerBulletL.Left - 6, playerBulletL.Right, playerBulletL.Top, playerBulletL.Bottom) == 0)
                        {
                            playerBulletL.Left = playerBulletL.Left - 1000;
                            enemyBulletU.Top = enemyBulletU.Top - 1000;
                        }
                        //enemyBulletD
                        else if (bulletCollision(playerBulletL.Left - 6, playerBulletL.Right, playerBulletL.Top, playerBulletL.Bottom) == 1)
                        {
                            playerBulletL.Left = playerBulletL.Left - 1000;
                            enemyBulletD.Top = enemyBulletD.Top + 1000;
                        }
                        //enemyBulletL
                        else if (bulletCollision(playerBulletL.Left - 6, playerBulletL.Right, playerBulletL.Top, playerBulletL.Bottom) == 2)
                        {
                            playerBulletL.Left = playerBulletL.Left - 1000;
                            enemyBulletL.Left = enemyBulletL.Left - 1000;
                        }
                        //enemyBulletR
                        else if (bulletCollision(playerBulletL.Left - 6, playerBulletL.Right, playerBulletL.Top, playerBulletL.Bottom) == 3)
                        {
                            playerBulletL.Left = playerBulletL.Left - 1000;
                            enemyBulletR.Left = enemyBulletR.Left + 1000;
                        }
                        //enemyDrone
                        else if (bulletCollision(playerBulletL.Left - 6, playerBulletL.Right, playerBulletL.Top, playerBulletL.Bottom) == 4)
                        {
                            playerBulletL.Left = playerBulletL.Left - 1000;
                            //Destroyed in two shots without an upgrade
                            if (bulletsUpgraded == false)
                            {
                                if (enemyDamaged == true)
                                {
                                    if (batteryUpgraded == false)
                                    {
                                        if (curBattery < 1000)
                                        {
                                            curBattery += 50;
                                        }
                                    }
                                    else
                                    {
                                        if (curBattery < 2000)
                                        {
                                            curBattery += 50;
                                        }
                                    }
                                    enemyActivea = false;
                                    enemyBulletU.Top = enemyBulletU.Top - 1000;
                                    enemyBulletD.Top = enemyBulletD.Top - 1000;
                                    enemyBulletL.Left = enemyBulletL.Top - 1000;
                                    enemyBulletR.Left = enemyBulletR.Top - 1500;
                                }
                                else
                                {
                                    enemyDamaged = true;
                                }
                            }
                            //Destroyed in one shot with an upgrade
                            else
                            {
                                if (batteryUpgraded == false)
                                {
                                    if (curBattery < 1000)
                                    {
                                        curBattery += 50;
                                    }
                                }
                                else
                                {
                                    if (curBattery < 2000)
                                    {
                                        curBattery += 50;
                                    }
                                }
                                enemyActivea = false;
                                enemyBulletU.Top = enemyBulletU.Top - 1000;
                                enemyBulletD.Top = enemyBulletD.Top - 1000;
                                enemyBulletL.Left = enemyBulletL.Top - 1000;
                                enemyBulletR.Left = enemyBulletR.Top - 1500;
                            }
                        }
                        //enemyBulletL2
                        else if (bulletCollision(playerBulletL.Left - 6, playerBulletL.Right, playerBulletL.Top, playerBulletL.Bottom) == 5)
                        {
                            playerBulletL.Left = playerBulletL.Left - 1000;
                            enemyBulletL2.Left = enemyBulletL2.Left - 1000;
                        }
                        //enemyBulletR2
                        else if (bulletCollision(playerBulletL.Left - 6, playerBulletL.Right, playerBulletL.Top, playerBulletL.Bottom) == 6)
                        {
                            playerBulletL.Left = playerBulletL.Left - 1000;
                            enemyBulletR2.Left = enemyBulletR2.Left + 1000;
                        }
                        //enemyDeliverer
                        else if (bulletCollision(playerBulletL.Left, playerBulletL.Right, playerBulletL.Top, playerBulletL.Bottom + 6) == 7)
                        {
                            playerBulletL.Left = playerBulletL.Left - 1000;
                            //Destroyed in two shots without an upgrade
                            if (bulletsUpgraded == false)
                            {
                                if (enemyDamagedB == true)
                                {
                                    if (batteryUpgraded == false)
                                    {
                                        if (curBattery < 1000)
                                        {
                                            curBattery += 50;
                                        }
                                    }
                                    else
                                    {
                                        if (curBattery < 2000)
                                        {
                                            curBattery += 50;
                                        }
                                    }
                                    enemyActiveb = false;
                                    enemyBulletL2.Left = enemyBulletL2.Top - 1000;
                                    enemyBulletR2.Left = enemyBulletR2.Top - 1500;
                                    curCash += 25;
                                    dayPts[curDay - 1] += 50;
                                    cashEarned[curDay - 1] += 25;
                                    lblCash.Text = "CASH: " + curCash;
                                }
                                else
                                {
                                    enemyDamagedB = true;
                                }
                            }
                            //Destroyed in one shot with an upgrade
                            else
                            {
                                if (batteryUpgraded == false)
                                {
                                    if (curBattery < 1000)
                                    {
                                        curBattery += 50;
                                    }
                                }
                                else
                                {
                                    if (curBattery < 2000)
                                    {
                                        curBattery += 50;
                                    }
                                }
                                enemyActiveb = false;
                                enemyBulletL2.Left = enemyBulletL2.Top - 1000;
                                enemyBulletR2.Left = enemyBulletR2.Top - 1500;
                                curCash += 25;
                                dayPts[curDay - 1] += 50;
                                cashEarned[curDay - 1] += 25;
                                lblCash.Text = "CASH: " + curCash;
                            }
                        }
                    }
                }
                //Player shooting right
                if (playerShootRight == true)
                {
                    playerBulletR.Left = playerBulletR.Left + 10;

                    //Bullet dissapearing allowing for another shot
                    if (playerBulletR.Right > 1500)
                    {
                        playerShoot = false;
                        playerShootRight = false;
                    }

                    //Detecting what object the bullet has collided with
                    else if (enemyCollision(playerBulletR.Left, playerBulletR.Right + 6, playerBulletR.Top, playerBulletR.Bottom))
                    {
                        //enemyBulletU
                        if (bulletCollision(playerBulletR.Left, playerBulletL.Right + 6, playerBulletR.Top, playerBulletR.Bottom) == 0)
                        {
                            playerBulletR.Left = playerBulletR.Left + 1000;
                            enemyBulletU.Top = enemyBulletU.Top - 1000;
                        }
                        //enemyBulletD
                        else if (bulletCollision(playerBulletR.Left, playerBulletR.Right + 6, playerBulletR.Top, playerBulletR.Bottom) == 1)
                        {
                            playerBulletR.Left = playerBulletR.Left + 1000;
                            enemyBulletD.Top = enemyBulletD.Top + 1000;
                        }
                        //enemyBulletL
                        else if (bulletCollision(playerBulletR.Left, playerBulletR.Right + 6, playerBulletR.Top, playerBulletR.Bottom) == 2)
                        {
                            playerBulletR.Left = playerBulletR.Left + 1000;
                            enemyBulletL.Left = enemyBulletL.Left - 1000;
                        }
                        //enemyBulletR
                        else if (bulletCollision(playerBulletR.Left, playerBulletR.Right + 6, playerBulletR.Top, playerBulletR.Bottom) == 3)
                        {
                            playerBulletR.Left = playerBulletR.Left + 1000;
                            enemyBulletR.Left = enemyBulletR.Left + 1000;
                        }
                        //enemyDrone
                        else if (bulletCollision(playerBulletR.Left, playerBulletR.Right + 6, playerBulletR.Top, playerBulletR.Bottom) == 4)
                        {
                            playerBulletR.Left = playerBulletR.Left + 1000;
                            //Destroyed in two shots without an upgrade
                            if (bulletsUpgraded == false)
                            {
                                if (enemyDamaged == true)
                                {
                                    enemyActivea = false;
                                    enemyBulletU.Top = enemyBulletU.Top - 1000;
                                    enemyBulletD.Top = enemyBulletD.Top - 1000;
                                    enemyBulletL.Left = enemyBulletL.Top - 1000;
                                    enemyBulletR.Left = enemyBulletR.Top - 1500;
                                    if (batteryUpgraded == false)
                                    {
                                        if (curBattery < 1000)
                                        {
                                            curBattery += 50;
                                        }
                                    }
                                    else
                                    {
                                        if (curBattery < 2000)
                                        {
                                            curBattery += 50;
                                        }
                                    }
                                }

                                else
                                {
                                    enemyDamaged = true;
                                }
                            }
                            //Destroyed in one shot with an upgrade
                            else
                            {
                                enemyActivea = false;
                                enemyBulletU.Top = enemyBulletU.Top - 1000;
                                enemyBulletD.Top = enemyBulletD.Top - 1000;
                                enemyBulletL.Left = enemyBulletL.Top - 1000;
                                enemyBulletR.Left = enemyBulletR.Top - 1500;
                                if (batteryUpgraded == false)
                                {
                                    if (curBattery < 1000)
                                    {
                                        curBattery += 50;
                                    }
                                }
                                else
                                {
                                    if (curBattery < 2000)
                                    {
                                        curBattery += 50;
                                    }
                                }
                            }    
                        }
                        //enemyBulletL2
                        else if (bulletCollision(playerBulletR.Left, playerBulletR.Right + 6, playerBulletR.Top, playerBulletR.Bottom) == 5)
                        {
                            playerBulletR.Left = playerBulletR.Left + 1000;
                            enemyBulletL2.Left = enemyBulletL2.Left - 1000;
                        }
                        //enemyBulletR2
                        else if (bulletCollision(playerBulletR.Left, playerBulletR.Right + 6, playerBulletR.Top, playerBulletR.Bottom) == 6)
                        {
                            playerBulletR.Left = playerBulletR.Left + 1000;
                            enemyBulletR2.Left = enemyBulletR2.Left + 1000;
                        }
                        //enemyDeliverer
                        else if (bulletCollision(playerBulletR.Left, playerBulletR.Right + 6, playerBulletR.Top, playerBulletR.Bottom) == 7)
                        {
                            playerBulletR.Left = playerBulletR.Left + 1000;
                            //Destroyed in two shots without an upgrade
                            if (bulletsUpgraded == false)
                            {
                                if (enemyDamagedB == true)
                                {
                                    if (batteryUpgraded == false)
                                    {
                                        if (curBattery < 1000)
                                        {
                                            curBattery += 50;
                                        }
                                    }
                                    else
                                    {
                                        if (curBattery < 2000)
                                        {
                                            curBattery += 50;
                                        }
                                    }
                                    curCash += 25;
                                    dayPts[curDay - 1] += 50;
                                    cashEarned[curDay - 1] += 25;
                                    enemyActiveb = false;
                                    enemyBulletL2.Left = enemyBulletL2.Top - 1000;
                                    enemyBulletR2.Left = enemyBulletR2.Top - 1500;
                                    curCash += 25;
                                    dayPts[curDay - 1] += 50;
                                    cashEarned[curDay - 1] += 25;
                                    lblCash.Text = "CASH: " + curCash;
                                }
                                else
                                {
                                    enemyDamagedB = true;
                                }
                            }
                            //Destroyed in one shot with an upgrade
                            else
                            {
                                if (batteryUpgraded == false)
                                {
                                    if (curBattery < 1000)
                                    {
                                        curBattery += 50;
                                    }
                                }
                                else
                                {
                                    if (curBattery < 2000)
                                    {
                                        curBattery += 50;
                                    }
                                }
                                curCash += 25;
                                dayPts[curDay - 1] += 50;
                                cashEarned[curDay - 1] += 25;
                                enemyActiveb = false;
                                enemyBulletL2.Left = enemyBulletL2.Top - 1000;
                                enemyBulletR2.Left = enemyBulletR2.Top - 1500;
                                curCash += 25;
                                dayPts[curDay - 1] += 50;
                                cashEarned[curDay - 1] += 25;
                                lblCash.Text = "CASH: " + curCash;
                            }
                        }
                    }
                }
            }
            //enemy shooting in the players direction while active
            if (enemyActivea == true)
            {
                if (enemyShoot == true)
                {
                    //enemy shoot up
                    if (enemyShootUp == true)
                    {
                        enemyBulletU.Top = enemyBulletU.Top - 5;
                        if (enemyBulletU.Bottom < 0)
                        {
                            enemyShoot = false;
                            enemyShootUp = false;
                        }
                    }
                    //enemy shoot down
                    if (enemyShootDown == true)
                    {
                        enemyBulletD.Top = enemyBulletD.Top + 5;
                        if (enemyBulletD.Top > 1000)
                        {
                            enemyShoot = false;
                            enemyShootDown = false;
                        }
                    }
                    //enemy shoot left
                    if (enemyShootLeft == true)
                    {
                        enemyBulletL.Left = enemyBulletL.Left - 5;
                        if (enemyBulletL.Right < 0)
                        {
                            enemyShoot = false;
                            enemyShootLeft = false;
                        }
                    }
                    //enemy shoot right
                    if (enemyShootRight == true)
                    {
                        enemyBulletR.Left = enemyBulletR.Left + 5;
                        if (enemyBulletR.Right > 1200)
                        {
                            enemyShoot = false;
                            enemyShootRight = false;
                        }
                    }
                }
            }
            //bonus enemy shooting in the players direction while active
            if (enemyActiveb == true)
            {
                if (enemyShootB == true)
                {
                    //bonus enemy shoot left
                    if (enemyShootLeftB == true)
                    {
                        enemyBulletL2.Left = enemyBulletL2.Left - 5;
                        if (enemyBulletL2.Right < 0)
                        {
                            enemyShootB = false;
                            enemyShootLeftB = false;
                        }
                    }
                    //bonus enemy shoot right
                    if (enemyShootRightB == true)
                    {
                        enemyBulletR2.Left = enemyBulletR2.Left + 5;
                        if (enemyBulletR2.Right > 1200)
                        {
                            enemyShootB = false;
                            enemyShootRightB = false;
                        }
                    }
                }
            }

            //house shoot furniture if angry
            if (houseShoot == true)
            {
                thrownFurniture.Top = thrownFurniture.Top - 25;
                if (thrownFurniture.Bottom < 0)
                {
                    houseShoot = false;
                }
                //Being hit by the furniture end the day immeditatley 
                else if (playerCollision(thrownFurniture.Left, thrownFurniture.Right, thrownFurniture.Top - 6, thrownFurniture.Bottom) == true)
                {
                    houseShoot = false;
                    thrownFurniture.Left -= 1000;
                    endDay();
                }
                //if an enemy is hit they are destroyed instantly
                else if (bulletCollision(thrownFurniture.Left, thrownFurniture.Right, thrownFurniture.Top - 6, thrownFurniture.Bottom) == 4)
                {
                    enemyActivea = false;
                    enemyBulletU.Top = enemyBulletU.Top - 1000;
                    enemyBulletD.Top = enemyBulletD.Top - 1000;
                    enemyBulletL.Left = enemyBulletL.Top - 1000;
                    enemyBulletR.Left = enemyBulletR.Top - 1500;
                }
                //if a bonus enemy is hit they are also destroyed instantly
                else if (bulletCollision(thrownFurniture.Left, thrownFurniture.Right, thrownFurniture.Top - 6, thrownFurniture.Bottom) == 7)
                {
                    enemyActiveb = false;
                    enemyBulletL2.Left = enemyBulletL2.Top - 1000;
                    enemyBulletR2.Left = enemyBulletR2.Top - 1500;
                    curCash += 25;
                    dayPts[curDay - 1] += 50;
                    cashEarned[curDay - 1] += 25;
                    lblCash.Text = "CASH: " + curCash;
                }
            }
        }

        //Responsible for the movement of the enemies and what directions they should shoot
        private void enemyTimer_Tick(object sender, EventArgs e)
        {
            if (enemyActivea == true)
            {
                //Enemy move up towards player
                if ((enemyDrone.Top > playerDrone.Top) && !droneCollisions(enemyDrone.Left, enemyDrone.Right, enemyDrone.Top-6, enemyDrone.Bottom))
                {
                    enemyDrone.Top = enemyDrone.Top - 1;
                    /*
                    if (enemyShootUp == false && enemyShoot == false)
                    {
                        enemyBulletU.Left = enemyDrone.Left;
                        enemyBulletU.Top = enemyDrone.Top;
                        enemyShoot = true;
                        enemyShootUp = true;
                    }
                    */
                }
                //Enemy move down towards player
                if ((enemyDrone.Top < playerDrone.Top)  && !droneCollisions(enemyDrone.Left, enemyDrone.Right, enemyDrone.Top, enemyDrone.Bottom+6))
                {
                    enemyDrone.Top = enemyDrone.Top + 1;
                    /*
                    if (enemyShootDown == false && enemyShoot == false)
                    {
                        enemyBulletD.Left = enemyDrone.Left;
                        enemyBulletD.Top = enemyDrone.Top;
                        enemyShoot = true;
                        enemyShootDown = true;
                    }
                    */
                }

                //Enemy move left towards player 
                if ((enemyDrone.Left > playerDrone.Left)  && !droneCollisions(enemyDrone.Left-6, enemyDrone.Right, enemyDrone.Top, enemyDrone.Bottom))
                {
                    enemyDrone.Left = enemyDrone.Left - 1;
                    if (enemyShootLeft == false && enemyShoot == false)
                    {
                        enemyBulletL.Left = enemyDrone.Left;
                        enemyBulletL.Top = enemyDrone.Top;
                        enemyShoot = true;
                        enemyShootLeft = true;
                    }
                }
                //Enemy move right towards player
                if ((enemyDrone.Left < playerDrone.Left)  && !droneCollisions(enemyDrone.Left, enemyDrone.Right+6, enemyDrone.Top, enemyDrone.Bottom))
                {
                    enemyDrone.Left = enemyDrone.Left + 1;
                    if (enemyShootRight == false && enemyShoot == false)
                    {
                        enemyBulletR.Left = enemyDrone.Left;
                        enemyBulletR.Top = enemyDrone.Top;
                        enemyShoot = true;
                        enemyShootRight = true;
                    }
                }
            }

            if (enemyActiveb == true)
            {
                //Bonus enemy move up towards player
                if ((enemyDeliverer.Top > playerDrone.Top) && !droneCollisions(enemyDeliverer.Left, enemyDeliverer.Right, enemyDeliverer.Top - 6, enemyDeliverer.Bottom))
                {
                    enemyDeliverer.Top = enemyDeliverer.Top - 1;
                }
                //Bonus enemy move dowm towards player
                if ((enemyDeliverer.Top < playerDrone.Top) && !droneCollisions(enemyDeliverer.Left, enemyDeliverer.Right, enemyDeliverer.Top, enemyDeliverer.Bottom + 6))
                {
                    enemyDeliverer.Top = enemyDeliverer.Top + 1;
                }
                //Bonus enemy move left towards player
                if ((enemyDeliverer.Left > playerDrone.Left) && !droneCollisions(enemyDeliverer.Left - 6, enemyDeliverer.Right, enemyDeliverer.Top, enemyDeliverer.Bottom))
                {
                    enemyDeliverer.Left = enemyDeliverer.Left - 1;
                    if (enemyShootLeftB == false && enemyShootB == false)
                    {
                        enemyBulletL2.Left = enemyDeliverer.Left;
                        enemyBulletL2.Top = enemyDeliverer.Top;
                        enemyShootB = true;
                        enemyShootLeftB = true;
                    }
                }
                //Bonus enemy move right towards player
                if ((enemyDeliverer.Left < playerDrone.Left) && !droneCollisions(enemyDeliverer.Left, enemyDeliverer.Right + 6, enemyDeliverer.Top, enemyDeliverer.Bottom))
                {
                    enemyDeliverer.Left = enemyDeliverer.Left + 1;
                    if (enemyShootRightB == false && enemyShootB == false)
                    {
                        enemyBulletR2.Left = enemyDeliverer.Left;
                        enemyBulletR2.Top = enemyDeliverer.Top;
                        enemyShootB = true;
                        enemyShootRightB = true;
                    }
                }
            }
        }

        private void gameHud_Click(object sender, EventArgs e)
        {

        }

        //Responsible for removing bullets off the screen if they happen to be shot, damages/destroys the parcel when shot =, and updates the enemy's look based on their health
        private void collisionTimer_Tick(object sender, EventArgs e)
        {
            if (enemyCollision(playerDrone.Left, playerDrone.Right, playerDrone.Top, playerDrone.Bottom))
            {
                enemyBulletU.Top = enemyBulletU.Top - 1000;
                enemyBulletD.Top = enemyBulletD.Top - 1000;
                enemyBulletL.Left = enemyBulletL.Top - 1000;
                enemyBulletR.Left = enemyBulletR.Top - 1500;

                enemyBulletL2.Left = enemyBulletL2.Top - 1000;
                enemyBulletR2.Left = enemyBulletR2.Top - 1500;

                //Parcel is damaged when shit if not damaged already
                if (parcelAttached == true && parcelDamaged == false)
                {
                    parcelDamaged = true;
                }
                //Destroys parcel that is damaged already
                else if (parcelAttached == true && parcelDamaged == true)
                {
                    parcelDamaged = false;
                    parcelBroken = true;
                    parcelAttached = false;
                    parcelFall = true;
                }
                else
                {
                    curBattery -= 1001;
                }
            }

            //Updeates parcels image representing its health
            if (parcelDamaged == true)
            {
                playerParcel.Image = parcelDamagedPic.Image;
            }
            else
            {
                playerParcel.Image = parcelFixedPic.Image;
            }

            //Updates enemies image representing its health
            if (enemyDamaged == true)
            {
                enemyDrone.Image = enemyDroneDamagedPic.Image;
            }
            else
            {
                enemyDrone.Image = enemyDroneFixedPic.Image;
            }

            //Updated bonus enemies image representing its health
            if (enemyDamagedB == true)
            {
                enemyDeliverer.Image = delivererDroneDamagedPic.Image;
            }
            else
            {
                enemyDeliverer.Image = delivererDroneFixedPic.Image;
            }
        }

        //Responsible for moving all of the drones in random directions
        private void windTimer_Tick(object sender, EventArgs e)
        {
            //Random numbers between 1 and 5 are selected
            playerWind = playerRng.Next(1, 5);
            enemyWind = enemyRng.Next(1, 5);
            delivererWind = delivererRng.Next(1, 5);

            if (curBattery > 0)
            {
                //player moves up
                if ((playerWind == 1) && (!droneCollisions(playerDrone.Left, playerDrone.Right, playerDrone.Top - 6, playerDrone.Bottom)))
                {
                    playerDrone.Top = playerDrone.Top - 2;
                    //parcel moves up
                    if ((parcelAttached == true) && (!droneCollisionsExcHouse(playerParcel.Left, playerParcel.Right, playerParcel.Top - 6, playerParcel.Bottom)))
                    {
                        playerParcel.Top = playerParcel.Top - 2;
                    }
                }
                //player moves down
                if ((playerWind == 2) && (!droneCollisions(playerDrone.Left, playerDrone.Right, playerDrone.Top, playerDrone.Bottom + 6)))
                {
                    playerDrone.Top = playerDrone.Top + 2;
                    //parcel moves down
                    if ((parcelAttached == true) && (!droneCollisionsExcHouse(playerParcel.Left, playerParcel.Right, playerParcel.Top, playerParcel.Bottom + 6)))
                    {
                        playerParcel.Top = playerParcel.Top + 2;
                    }
                }
                //player moves left
                if ((playerWind == 3) && (!droneCollisions(playerDrone.Left - 6, playerDrone.Right, playerDrone.Top, playerDrone.Bottom)))
                {
                    if (playerDrone.Left > this.Width - 1000)
                    {
                        playerDrone.Left = playerDrone.Left - 2;
                    }
                    //parcel moves left
                    if ((parcelAttached == true) && (!droneCollisionsExcHouse(playerParcel.Left - 6, playerParcel.Right, playerParcel.Top, playerParcel.Bottom)) && !(playerParcel.Left < this.Width - 1000))
                    {
                        playerParcel.Left = playerParcel.Left - 2;
                    }
                }
                //player moves right
                if ((playerWind == 4) && (!droneCollisions(playerDrone.Left, playerDrone.Right + 6, playerDrone.Top, playerDrone.Bottom)))
                {
                    if (playerDrone.Right < this.Width - 30)
                    {
                        playerDrone.Left = playerDrone.Left + 2;
                    }
                    //parcel moves right
                    if ((parcelAttached == true) && (!droneCollisionsExcHouse(playerParcel.Left, playerParcel.Right+6, playerParcel.Top, playerParcel.Bottom)) && !(playerParcel.Right > this.Width - 30))
                    {
                        playerParcel.Left = playerParcel.Left + 2;
                    }
                }
            }

            if (enemyActivea == true)
            {
                //enemy moves up
                if ((enemyWind == 1) && (!droneCollisions(enemyDrone.Left, enemyDrone.Right, enemyDrone.Top - 6, enemyDrone.Bottom)))
                {
                    enemyDrone.Top = enemyDrone.Top - 2;
                }
                //enemy moves down
                if ((enemyWind == 2) && (!droneCollisions(enemyDrone.Left, enemyDrone.Right, enemyDrone.Top, enemyDrone.Bottom + 6)))
                {
                    enemyDrone.Top = enemyDrone.Top + 2;
                }
                //enemy moves left
                if ((enemyWind == 3) && (!droneCollisions(enemyDrone.Left - 6, enemyDrone.Right, enemyDrone.Top, enemyDrone.Bottom)) && !(enemyDrone.Left < this.Width - 1000))
                {
                    enemyDrone.Left = enemyDrone.Left - 2;
                }
                //enemy moves right
                if ((enemyWind == 4) && (!droneCollisions(enemyDrone.Left, enemyDrone.Right + 6, enemyDrone.Top, enemyDrone.Bottom)) && !(enemyDrone.Left > this.Width - 30))
                {
                    enemyDrone.Left = enemyDrone.Left + 2;
                }
            }
            if (enemyActiveb == true)
            {
                //bonus enemy moves up
                if ((delivererWind == 1) && (!droneCollisions(enemyDeliverer.Left, enemyDeliverer.Right, enemyDeliverer.Top - 6, enemyDeliverer.Bottom)))
                {
                    enemyDeliverer.Top = enemyDeliverer.Top - 2;
                }
                //bonus enemy moves down
                if ((delivererWind == 2) && (!droneCollisions(enemyDeliverer.Left, enemyDeliverer.Right, enemyDeliverer.Top, enemyDeliverer.Bottom + 6)))
                {
                    enemyDeliverer.Top = enemyDeliverer.Top + 2;
                }
                //bonus enemy moves left
                if ((delivererWind == 3) && (!droneCollisions(enemyDeliverer.Left - 6, enemyDeliverer.Right, enemyDeliverer.Top, enemyDeliverer.Bottom)) && !(enemyDeliverer.Left < this.Width - 1000))
                {
                    enemyDeliverer.Left = enemyDeliverer.Left - 2;
                }
                //bonus enemy moves right
                if ((delivererWind == 4) && (!droneCollisions(enemyDeliverer.Left, enemyDeliverer.Right + 6, enemyDeliverer.Top, enemyDeliverer.Bottom)) && !(enemyDeliverer.Left > this.Width - 30))
                {
                    enemyDeliverer.Left = enemyDeliverer.Left + 2;
                }
            }
        }

        //Responsible for the response of the house throwing furniture if the player deliveres to the wrong house
        private void angerTimer_Tick(object sender, EventArgs e)
        {
            PictureBox[] angryHouses = { angryHouse1, angryHouse2, angryHouse3 };
            PictureBox[] curHouses = { house1, house2, house3 };

            if (angryHouse == true && angryHouses[curHouse].Visible == false)
            {
                angryHouses[curHouse].Visible = true;
            }
            else if (angryHouse == true && angryHouses[curHouse].Visible == true)
            {
                angryHouses[curHouse].Visible = false;
                angryHouse = false;

                houseShoot = true;

                thrownFurniture.Top = curHouses[curHouse].Bottom;
                thrownFurniture.Left = curHouses[curHouse].Left;
            }
        }

        //Responsible for moving drones and parcels left or right at different forces depending on the value of windDay[]
        private void windyTimer_Tick(object sender, EventArgs e)
        {

            if (windDay[curDay - 1] == 1) //weak left wind
            {
                windArrow.Image = greenArrowL.Image;
                if (enemyActivea == true && (!droneCollisions(enemyDrone.Left - 6, enemyDrone.Right, enemyDrone.Top, enemyDrone.Bottom)) && !(enemyDrone.Left < this.Width - 1000))
                {
                    enemyDrone.Left = enemyDrone.Left - 1;
                }
                if (enemyActiveb == true && (!droneCollisions(enemyDeliverer.Left - 6, enemyDeliverer.Right, enemyDeliverer.Top, enemyDeliverer.Bottom)) && !(enemyDeliverer.Left < this.Width - 1000))
                {
                    enemyDeliverer.Left = enemyDeliverer.Left - 1;
                }
                if (droneUpgraded == false)
                {
                    if (!droneCollisions(playerDrone.Left - 6, playerDrone.Right, playerDrone.Top, playerDrone.Bottom) && !(playerDrone.Left < this.Width - 1000))
                    {
                        playerDrone.Left = playerDrone.Left - 1;
                    }
                    if ((parcelAttached == true) && (!droneCollisionsExcHouse(playerParcel.Left - 6, playerParcel.Right, playerParcel.Top, playerParcel.Bottom)) && !(playerParcel.Left < this.Width - 1000))
                    {
                        playerParcel.Left = playerParcel.Left - 1;
                    }
                }

            }
            if (windDay[curDay - 1] == 2) //weak right wind
            {
                windArrow.Image = greenArrowR.Image;
                if (enemyActivea == true && (!droneCollisions(enemyDrone.Left, enemyDrone.Right + 6, enemyDrone.Top, enemyDrone.Bottom)) && !(enemyDrone.Right > this.Width - 30))
                {
                    enemyDrone.Left = enemyDrone.Left + 1;
                }
                if (enemyActiveb == true && (!droneCollisions(enemyDeliverer.Left, enemyDeliverer.Right + 6, enemyDeliverer.Top, enemyDeliverer.Bottom)) && !(enemyDeliverer.Right > this.Width - 30))
                {
                    enemyDeliverer.Left = enemyDeliverer.Left + 1;
                }
                if (droneUpgraded == false)
                {
                    if (!droneCollisions(playerDrone.Left, playerDrone.Right + 6, playerDrone.Top, playerDrone.Bottom) && !(playerDrone.Right > this.Width - 30))
                    {
                        playerDrone.Left = playerDrone.Left + 1;
                    }
                    if ((parcelAttached == true) && (!droneCollisionsExcHouse(playerParcel.Left, playerParcel.Right + 6, playerParcel.Top, playerParcel.Bottom)) && !(playerParcel.Right > this.Width - 30))
                    {
                        playerParcel.Left = playerParcel.Left + 1;
                    }
                }

            }
            if (windDay[curDay - 1] == 3) //medium left wind
            {
                windArrow.Image = orangeArrowL.Image;
                if (enemyActivea == true && (!droneCollisions(enemyDrone.Left - 6, enemyDrone.Right, enemyDrone.Top, enemyDrone.Bottom)) && !(enemyDrone.Left < this.Width - 1000))
                {
                    enemyDrone.Left = enemyDrone.Left - 2;
                }
                if (enemyActiveb == true && (!droneCollisions(enemyDeliverer.Left - 6, enemyDeliverer.Right, enemyDeliverer.Top, enemyDeliverer.Bottom)) && !(enemyDeliverer.Left < this.Width - 1000))
                {
                    enemyDeliverer.Left = enemyDeliverer.Left - 2;
                }
                if (droneUpgraded == false)
                {
                    if (!droneCollisions(playerDrone.Left - 6, playerDrone.Right, playerDrone.Top, playerDrone.Bottom) && !(playerDrone.Left < this.Width - 1000))
                    {
                        playerDrone.Left = playerDrone.Left - 2;
                    }
                    if ((parcelAttached == true) && (!droneCollisionsExcHouse(playerParcel.Left - 6, playerParcel.Right, playerParcel.Top, playerParcel.Bottom)) && !(playerParcel.Left < this.Width - 1000))
                    {
                        playerParcel.Left = playerParcel.Left - 2;
                    }
                }

            }
            if (windDay[curDay - 1] == 4) //medium right wind
            {
                windArrow.Image = orangeArrowR.Image;
                if (enemyActivea == true && (!droneCollisions(enemyDrone.Left, enemyDrone.Right + 6, enemyDrone.Top, enemyDrone.Bottom)) && !(enemyDrone.Right > this.Width - 30))
                {
                    enemyDrone.Left = enemyDrone.Left + 2;
                }
                if (enemyActiveb == true && (!droneCollisions(enemyDeliverer.Left, enemyDeliverer.Right + 6, enemyDeliverer.Top, enemyDeliverer.Bottom)) && !(enemyDeliverer.Right > this.Width - 30))
                {
                    enemyDeliverer.Left = enemyDeliverer.Left + 2;
                }
                if (droneUpgraded == false)
                {
                    if (!droneCollisions(playerDrone.Left, playerDrone.Right + 6, playerDrone.Top, playerDrone.Bottom) && !(playerDrone.Right > this.Width - 30))
                    {
                        playerDrone.Left = playerDrone.Left + 2;
                    }
                    if ((parcelAttached == true) && (!droneCollisionsExcHouse(playerParcel.Left, playerParcel.Right + 6, playerParcel.Top, playerParcel.Bottom)) && !(playerParcel.Right > this.Width - 30))
                    {
                        playerParcel.Left = playerParcel.Left + 2;
                    }
                }

            }
            if (windDay[curDay - 1] == 5) //strong left wind
            {
                windArrow.Image = redArrowL.Image;
                if (enemyActivea == true && (!droneCollisions(enemyDrone.Left - 6, enemyDrone.Right, enemyDrone.Top, enemyDrone.Bottom)) && !(enemyDrone.Left < this.Width - 1000))
                {
                    enemyDrone.Left = enemyDrone.Left - 3;
                }
                if (enemyActiveb == true && (!droneCollisions(enemyDeliverer.Left - 6, enemyDeliverer.Right, enemyDeliverer.Top, enemyDeliverer.Bottom)) && !(enemyDeliverer.Left < this.Width - 1000))
                {
                    enemyDeliverer.Left = enemyDeliverer.Left - 3;
                }
                if (droneUpgraded == false)
                {
                    if (!droneCollisions(playerDrone.Left - 6, playerDrone.Right, playerDrone.Top, playerDrone.Bottom) && !(playerDrone.Left < this.Width - 1000))
                    {
                        playerDrone.Left = playerDrone.Left - 3;
                    }
                    if ((parcelAttached == true) && (!droneCollisionsExcHouse(playerParcel.Left - 6, playerParcel.Right, playerParcel.Top, playerParcel.Bottom)) && !(playerParcel.Left < this.Width - 1000))
                    {
                        playerParcel.Left = playerParcel.Left - 3;
                    }
                }

            }
            if (windDay[curDay - 1] == 6) //strong right wind
            {
                windArrow.Image = redArrowR.Image;
                if (enemyActivea == true && (!droneCollisions(enemyDrone.Left, enemyDrone.Right + 6, enemyDrone.Top, enemyDrone.Bottom)) && !(enemyDrone.Right > this.Width - 30))
                {
                    enemyDrone.Left = enemyDrone.Left + 3;
                }
                if (enemyActiveb == true && (!droneCollisions(enemyDeliverer.Left, enemyDeliverer.Right + 6, enemyDeliverer.Top, enemyDeliverer.Bottom)) && !(enemyDeliverer.Right > this.Width - 30))
                {
                    enemyDeliverer.Left = enemyDeliverer.Left + 3;
                }
                if (droneUpgraded == false)
                {
                    if (!droneCollisions(playerDrone.Left, playerDrone.Right + 6, playerDrone.Top, playerDrone.Bottom) && !(playerDrone.Right > this.Width - 30))
                    {
                        playerDrone.Left = playerDrone.Left + 3;
                    }
                    if ((parcelAttached == true) && (!droneCollisionsExcHouse(playerParcel.Left, playerParcel.Right + 6, playerParcel.Top, playerParcel.Bottom)) && !(playerParcel.Right > this.Width - 30))
                    {
                        playerParcel.Left = playerParcel.Left + 3;
                    }
                }

            }
        }

        //Responsible for spawning both the standard enemy and the bonus enemy, with a 10% chance to spawn each second and a 50% chance to spawn on either side
        private void spawnTimer_Tick(object sender, EventArgs e)
        {
            if ((curRoad == 0 || curRoad == 2) && enemyActivea == false)
            {
                enemySpawner = spawnEnemy.Next(0, 20);
                //spawn enemy on the left
                if (enemySpawner == 1)
                {
                    enemyDrone.Visible = true;
                    enemyDrone.Left = 12;
                    enemyDrone.Top = 107;
                    enemyFallSpeed = 1;
                    enemyDamaged = false;
                    enemyActivea = true;
                    enemyShoot = false;
                    enemyShootUp = false;
                    enemyShootDown = false;
                    enemyShootLeft = false;
                    enemyShootRight = false;
                }
                //spawn enemy on the right
                if (enemySpawner == 2)
                {
                    enemyDrone.Visible = true;
                    enemyDamaged = false;
                    enemyActivea = true;
                    enemyDrone.Left = 900;
                    enemyDrone.Top = 118;
                    enemyFallSpeed = 1;
                    enemyDamaged = false;
                    enemyActivea = true;
                    enemyShoot = false;
                    enemyShootUp = false;
                    enemyShootDown = false;
                    enemyShootLeft = false;
                    enemyShootRight = false;
                }
            }
            else if (curRoad == 3 && enemyActiveb == false)
            {
                enemySpawner = spawnEnemy.Next(0, 20);
                //spawn bonus enemy on the left
                if (enemySpawner == 1)
                {
                    enemyDeliverer.Visible = true;
                    enemyDeliverer.Left = 56;
                    enemyDeliverer.Top = 185;
                    enemyFallSpeedB = 1;
                    enemyDamagedB = false;
                    enemyActiveb = true;
                    enemyShootB = false;
                    enemyShootUpB = false;
                    enemyShootDownB = false;
                    enemyShootLeftB = false;
                    enemyShootRightB = false;
                }
                //spawn bonus enemy on the right
                if (enemySpawner == 2)
                {
                    enemyDeliverer.Visible = true;
                    enemyDeliverer.Left = 900;
                    enemyDeliverer.Top = 158;
                    enemyFallSpeedB = 1;
                    enemyDamagedB = false;
                    enemyActiveb = true;
                    enemyShootB = false;
                    enemyShootUpB = false;
                    enemyShootDownB = false;
                    enemyShootLeftB = false;
                    enemyShootRightB = false;
                }
            }
        }
    }
}
