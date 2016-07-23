﻿using System;
using CS;
using CS.Components;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

class Player
{
	public Fixture leg;
	public Body body;
	public int isTouching = 0;
	public Body side;

	public Player(World world, Body body)
	{
		leg = FixtureFactory.AttachRectangle(ConvertUnits.ToSimUnits(100-4), ConvertUnits.ToSimUnits(1), 1f, new Vector2(0, ConvertUnits.ToSimUnits(32+16+1)), body, false);
		leg.IsSensor = true;
		leg.OnCollision = collision;
		leg.OnSeparation = seperation;
		this.body = body;
		body.FixedRotation = true;
		body.SleepingAllowed = false;
		body.Friction = 0.8f;
		body.Mass = 1;

		side = BodyFactory.CreateRectangle(world, ConvertUnits.ToSimUnits(102), ConvertUnits.ToSimUnits(10), 1);
		side.IsStatic = false;
		side.GravityScale = 0;
		side.Mass = 0;
		side.Friction = 0;
		//leftSide.IgnoreCollisionWith(body);
		side.SleepingAllowed = false;

		//Joint j = new Jo
		JointFactory.CreateWeldJoint(world, body, side, body.Position, body.Position);
	}
	bool collision(Fixture a, Fixture b, Contact contact)
	{
		isTouching++;
		a.UserData = true;
		return true;
	}
	void seperation(Fixture a, Fixture b)
	{
		isTouching--;
		a.UserData = false;
	}
}

class PlayerSystem : ComponentSystem<Player>, ISysUpdateable
{
	public PlayerSystem(State state) : base(state)
	{
	}

	public void Update(Global G)
	{
		for(int i = 0; i < size; ++i)
		{
			if (entityIDs[i] == -1)
				continue;

			var player = components[i];
			var keyState = G.keyboardState;
			var vel = player.body.LinearVelocity;
			bool moving = false;
			var speed = ConvertUnits.ToSimUnits(230);

			if (keyState.IsKeyDown(Keys.A) && vel.X > -speed)
			{
				vel.X += -speed;
				moving = true;
			}
			if (keyState.IsKeyDown(Keys.D) && vel.X < speed)
			{
				vel.X += speed;
				moving = true;
			}

			

			var isTouching = player.leg.UserData as bool?;
			if(keyState.IsKeyDown(Keys.Space) && player.isTouching > 0)
			{
				vel.Y = -ConvertUnits.ToSimUnits(500);
				moving = true;

			}

			player.body.LinearVelocity = vel;
			//player.side.Position = player.body.Position;
		}
	}
}