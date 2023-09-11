﻿using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using server.DTOs;
using server.Entities;
using server.Interface;
using server.Utilities;

namespace server.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    private bool UserEntityExists(long id)
    {
        return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
    }

    public async Task<bool> Create(CreateUserRequest body)
    {
        UserEntity userEntity = new UserEntity
        {
            //Birth Entity 수정필요 (string type)
            Birth = body.Birth,
            Email = body.Email,
            Level = 0,
            Password = Password.SHA512(body.Password),
            Phone = body.Phone,
            ProfileURL = null,
            Sns = "",
            Username = body.Username
        };
        this.mContext.Users.Add(userEntity);
        await this.mContext.SaveChangesAsync();
        return true;
    }

    public async Task<GetUserResponse?> Read(long id)
    {
        UserEntity? user = await this._context.Users.FindAsync(id);

        if (user == null)
        {
            return null;
        }

        var response = new GetUserResponse
        {
            email = user.Email,
            username = user.Username,
            phone = user.Phone,
            birth = user.Birth,
            profile_url = user.ProfileURL,
            level = user.Level,
            sns = user.Sns.Split(',')
        };
        return response;
    }

    public async Task<bool> Update(long id, PutUserRequest model)
    {
        UserEntity? user = await this._context.Users.FindAsync(id);

        if (user == null)
        {
            return false;
        }

        user.Username = model.username ?? user.Username;
        user.Password = model.password ?? user.Password;
        user.Phone = model.phone ?? user.Phone;
        user.Birth = model.birth ?? user.Birth;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task Delete(long id)
    {
        UserEntity? user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            throw new Exception("The user with that ID does not exist.");
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }
}
