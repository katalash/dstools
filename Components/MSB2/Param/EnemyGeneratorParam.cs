using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using System.Numerics;
using System;

// Enemy Generator param
public class EnemyGeneratorParam : MonoBehaviour
{
    public long ID;

    // Location params
    public uint LocationUnk0C;
    public uint LocationUnk14;
    public uint LocationUnk18;
    public uint LocationUnk1C;

    // Generator params
    public byte Unk00;
    public byte Unk01;
    public byte Unk02;
    public byte Unk03;
    public uint Unk04;
    public uint GeneratorRegistrationParam;
    public byte Unk0C;
    public byte Unk0D;
    public byte Unk0E;
    public byte Unk0F;
    public uint Unk10;
    public uint Unk14;
    public uint Unk18;
    public uint Unk1C;
    public uint Unk20;
    public uint Unk24;
    public uint Unk28;
    public uint Unk2C;
    public uint Unk30;
    public uint Unk34;
    public uint Unk38;
    public uint Unk3C;
    public uint Unk40;
    public uint Unk44;
    public uint Unk48;
    public uint Unk4C;
    public uint Unk50;
    public uint Unk54;
    public uint Unk58;
    public uint Unk5C;
    public uint Unk60;
    public uint Unk64;
    public uint Unk68;
    public uint Unk6C;
    public uint Unk70;
    public uint Unk74;
    public uint Unk78;
    public uint Unk7C;
    public float Unk80;
    public uint Unk84;
    public uint Unk88;
    public uint Unk8C;
    public uint Unk90;
    public uint Unk94;
    public uint Unk98;
    public uint Unk9C;

    public void SetFromGeneratorParam(long id, PARAM.Row generator, PARAM.Row location)
    {
        ID = id;
        LocationUnk0C = (uint)location["Unk0C"].Value;
        LocationUnk14 = (uint)location["Unk14"].Value;
        LocationUnk18 = (uint)location["Unk18"].Value;
        LocationUnk1C = (uint)location["Unk1C"].Value;

        Unk00 = (byte)generator["Unk00"].Value;
        Unk01 = (byte)generator["Unk01"].Value;
        Unk02 = (byte)generator["Unk02"].Value;
        Unk03 = (byte)generator["Unk03"].Value;
        Unk04 = (uint)generator["Unk04"].Value;
        GeneratorRegistrationParam = (uint)generator["GeneratorRegistParam"].Value;
        Unk0C = (byte)generator["Unk0C"].Value;
        Unk0D = (byte)generator["Unk0D"].Value;
        Unk0E = (byte)generator["Unk0E"].Value;
        Unk0F = (byte)generator["Unk0F"].Value;
        Unk10 = (uint)generator["Unk10"].Value;
        Unk14 = (uint)generator["Unk14"].Value;
        Unk18 = (uint)generator["Unk18"].Value;
        Unk1C = (uint)generator["Unk1C"].Value;
        Unk20 = (uint)generator["Unk20"].Value;
        Unk24 = (uint)generator["Unk24"].Value;
        Unk28 = (uint)generator["Unk28"].Value;
        Unk2C = (uint)generator["Unk2C"].Value;
        Unk30 = (uint)generator["Unk30"].Value;
        Unk34 = (uint)generator["Unk34"].Value;
        Unk38 = (uint)generator["Unk38"].Value;
        Unk3C = (uint)generator["Unk3C"].Value;
        Unk40 = (uint)generator["Unk40"].Value;
        Unk44 = (uint)generator["Unk44"].Value;
        Unk48 = (uint)generator["Unk48"].Value;
        Unk4C = (uint)generator["Unk4C"].Value;
        Unk50 = (uint)generator["Unk50"].Value;
        Unk54 = (uint)generator["Unk54"].Value;
        Unk58 = (uint)generator["Unk58"].Value;
        Unk5C = (uint)generator["Unk5C"].Value;
        Unk60 = (uint)generator["Unk60"].Value;
        Unk64 = (uint)generator["Unk64"].Value;
        Unk68 = (uint)generator["Unk68"].Value;
        Unk6C = (uint)generator["Unk6C"].Value;
        Unk70 = (uint)generator["Unk70"].Value;
        Unk74 = (uint)generator["Unk74"].Value;
        Unk78 = (uint)generator["Unk78"].Value;
        Unk7C = (uint)generator["Unk7C"].Value;
        Unk80 = (float)generator["Unk80"].Value;
        Unk84 = (uint)generator["Unk84"].Value;
        Unk88 = (uint)generator["Unk88"].Value;
        Unk8C = (uint)generator["Unk8C"].Value;
        Unk90 = (uint)generator["Unk90"].Value;
        Unk94 = (uint)generator["Unk94"].Value;
        Unk98 = (uint)generator["Unk98"].Value;
        Unk9C = (uint)generator["Unk9C"].Value;
    }


    public void Serialize(PARAM.Row generator, PARAM.Row genLocation, GameObject parent)
    {
    }
}
