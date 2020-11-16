using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace myfirstapp
{
    public class Player
    {
        public int life { get; set; }
        public int x { get; set; }

        public Player()
        {
            life = 3;
            x = Console.WindowWidth / 2;
        }

        public void move()
        {
            if (Console.KeyAvailable)
            {
                ConsoleKey key = Console.ReadKey(true).Key;
                if (key.Equals(ConsoleKey.A))
                {
                    left();
                }
                if (key.Equals(ConsoleKey.D))
                {
                    right();
                }
            }
        }

        private void left()
        {
            x = Math.Max(0, x - 1);
        }

        private void right()
        {
            x = Math.Min(Console.WindowWidth, x + 1);
        }
    }

    public abstract class Enemy
    {
        public int x { get; set; }
        public int y { get; set; }

        public Enemy()
        {
            x = randomInteger();
            y = 0;
        }

        public abstract void fall();

        public abstract void update();

        public abstract void drawNewPosition();

        public abstract void deleteOldPosition();

        public int randomInteger()
        {
            Random random = new Random();
            return random.Next(0, Console.WindowWidth);
        }
    }

    public class NormalEnemy : Enemy
    {
        public override void fall()
        {
            y += 1;
        }

        public override void update()
        {
            if (y < Console.WindowHeight)
            {
                fall();
                drawNewPosition();
                deleteOldPosition();
            }
            else
            {
                Console.SetCursorPosition(x, y);
                Console.Write(" ");
                x = randomInteger();
                y = 0;
            }
        }

        public override void drawNewPosition()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.SetCursorPosition(x, y);
            Console.Write("E");
        }

        public override void deleteOldPosition()
        {
            Console.SetCursorPosition(x, y - 1);
            Console.Write(" ");
        }
    }

    public class MovingEnemy : Enemy
    {
        private int oldXDirection;
        private bool outOfBounds;
        public override void fall()
        {
            y += 1;
            oldXDirection = randomDirection();
            if ((x + oldXDirection < Console.WindowWidth) && (x + oldXDirection >= 0))
            {
                outOfBounds = false;
                x += oldXDirection;
            }
            else
            {
                outOfBounds = true;
            }
        }

        public override void update()
        {
            if (y < Console.WindowHeight)
            {
                fall();
                drawNewPosition();
                
            }
            else
            {
                Console.SetCursorPosition(x, y);
                Console.Write(" ");
                x = randomInteger();
                y = 0;
            }
        }

        public override void drawNewPosition()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(x, y);
            Console.Write("M");
        }

        public override void deleteOldPosition()
        {
            if (outOfBounds)
            {
                Console.SetCursorPosition(x, y - 1);
                outOfBounds = false;
            }
            else
            {
                Console.SetCursorPosition(x - oldXDirection, y - 1);
            }
            Console.Write(" ");
        }

        private int randomDirection()
        {
            Random random = new Random();
            int distance = random.Next(0, 2);
            if (distance == 0)
            {
                return -1;
            }
            else
            {
                return distance;
            }
        }
    }

    public class ShootingEnemy : Enemy
    {
        private Bullet bullet { get; set; }
        public override void fall()
        {
            y += 1;
        }

        public override void update()
        {
            if (y < Console.WindowHeight)
            {
                fall();
                drawNewPosition();
                deleteOldPosition();
            }
            else
            {
                Console.SetCursorPosition(x, y);
                Console.Write(" ");
                bullet = null;
                x = randomInteger();
                y = 0;
            }
        }

        public override void drawNewPosition()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.SetCursorPosition(x, y);
            Console.Write("S");
        }

        public override void deleteOldPosition()
        {
            Console.SetCursorPosition(x, y - 1);
            Console.Write(" ");
        }

        public Bullet shootBullet()
        {
            if (bullet == null)
            {
                bullet = new Bullet(this);
                return bullet;
            }
            return null;
        }

    }

    public class Bullet : Enemy
    {
        public Bullet(ShootingEnemy shootingEnemy)
        {
            x = shootingEnemy.x;
            y = shootingEnemy.y + 2;
        }

        public override void update()
        {
            if (y < Console.WindowHeight)
            {
                fall();
                drawNewPosition();
                deleteOldPosition();
            }
        }

        public void bottomDelete()
        {
            Console.SetCursorPosition(x, Console.WindowHeight - 1);
            Console.Write(" ");

            Console.SetCursorPosition(x, Console.WindowHeight);
            Console.Write(" ");
        }


        public override void fall()
        {
            if (y + 2 >= Console.WindowHeight)
            {
                y = Console.WindowHeight;
            }
            else
            {
                y += 2;
            }
        }

        public override void drawNewPosition()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.SetCursorPosition(x, y);
            Console.Write(".");
        }

        public override void deleteOldPosition()
        {
            Console.SetCursorPosition(x, y - 2);
            Console.Write(" ");

            if (y == Console.WindowHeight)
            {
                bottomDelete();
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Player player = new Player();
            bool gameOver = false;
            while (!gameOver)
            {
                List<Enemy> enemies = startUp(player);
                List<Bullet> bullets = new List<Bullet>();
                bool playerDied = false;
                while (!playerDied)
                {
                    foreach (Enemy enemy in enemies)
                    {
                        enemy.update();

                        if (enemy.GetType() == typeof(ShootingEnemy))
                        {
                            ShootingEnemy shootingEnemy = (ShootingEnemy)enemy;
                            Bullet bullet = shootingEnemy.shootBullet();
                            if (!(bullet == null))
                            {
                                bullets.Add(bullet);
                            }
                        }
                    }

                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.SetCursorPosition(0, 1);
                    Console.Write(player.life);

                    for (int i = 0; i < bullets.Count; i++)
                    {
                        Bullet bullet = bullets[i];
                        bullet.update();
                        if (bullet.y == Console.WindowHeight)
                        {
                            bullets.RemoveAt(i);
                            if (player.x == bullet.x)
                            {
                                playerDied = playerHit(player);
                            }
                        }
                    }

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.SetCursorPosition(player.x, Console.WindowHeight);
                    Console.Write("P");
                    System.Threading.Thread.Sleep(50);
                    Console.SetCursorPosition(player.x, Console.WindowHeight);
                    Console.Write(" ");
                    player.move();

                    foreach (Enemy enemy in enemies)
                    {
                        if (enemy.y == Console.WindowHeight)
                        {
                            if (player.x == enemy.x)
                            {
                                playerDied = playerHit(player);
                            }
                        }
                    }
                }

                if (player.life <= 0)
                {
                    gameOver = true;
                }
            }
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.SetCursorPosition(0, 1);
            Console.Write(0);
            Console.SetCursorPosition(Console.WindowWidth / 2 - 4, Console.WindowHeight / 2);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Game Over!");
            System.Threading.Thread.Sleep(30000);
        }

        public static List<Enemy> startUp(Player player)
        {
            Console.Clear();
            Console.CursorVisible = false;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.SetCursorPosition(player.x, Console.WindowHeight);
            Console.Write("P");
            Console.ForegroundColor = ConsoleColor.Gray;

            Console.SetCursorPosition(0, 1);
            Console.Write(player.life);

            List<Enemy> enemies = new List<Enemy>();
            enemies.Add(new NormalEnemy());
            enemies.Add(new NormalEnemy());
            enemies.Add(new NormalEnemy());
            enemies.Add(new NormalEnemy());
            enemies.Add(new NormalEnemy());
            enemies.Add(new NormalEnemy());
            enemies.Add(new MovingEnemy());
            enemies.Add(new MovingEnemy());
            enemies.Add(new MovingEnemy());
            enemies.Add(new MovingEnemy());
            enemies.Add(new MovingEnemy());
            enemies.Add(new MovingEnemy());
            enemies.Add(new MovingEnemy());
            enemies.Add(new ShootingEnemy());
            enemies.Add(new ShootingEnemy());
            enemies.Add(new ShootingEnemy());
            enemies.Add(new ShootingEnemy());
            enemies.Add(new ShootingEnemy());
            enemies.Add(new ShootingEnemy());
            enemies.Add(new ShootingEnemy());
            enemies.Add(new ShootingEnemy());

            return enemies;
        }

        public static bool playerHit(Player player)
        {
            Console.SetCursorPosition(Console.WindowWidth / 2 - 4, Console.WindowHeight / 2);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("You Died!");
            System.Threading.Thread.Sleep(300);
            Console.ForegroundColor = ConsoleColor.Blue;
            player.life -= 1;
            player.x = Console.WindowWidth / 2;
            return true;

        }
    }
}