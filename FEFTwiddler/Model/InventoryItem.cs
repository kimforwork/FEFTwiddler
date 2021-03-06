﻿using System;
using FEFTwiddler.Extensions;

namespace FEFTwiddler.Model
{
    public class InventoryItem : Item
    {
        public InventoryItem(byte[] raw) : base(raw)
        {
            if (raw.Length != 4) throw new ArgumentException("Inventory items must be 4 bytes");
        }

        public static InventoryItem FromID(Enums.Item itemId)
        {
            var raw = new byte[] { (byte)itemId, (byte)((ushort)itemId >> 8), 0x00, 0x00 };
            return new InventoryItem(raw);
        }

        /// <summary>
        /// A reference to the name of the weapon in the weapon name block.
        /// This value is two greater than the corresponding ID in the weapon name block for some reason (probably to differentiate it from 0).
        /// This returns the value as seen in the weapon name block (i.e. two is subtracted from it on get).
        /// </summary>
        public byte WeaponNameID
        {
            get { return (byte)(_raw[2] - 2); }
            set { _raw[2] = (byte)(value + 2); }
        }
        public bool IsNamed
        {
            get
            {
                // In JRCrichton's save (issue #50), this value is set to 01.
                // To my knowledge, 01 is an invalid value that can only be obtained by hex editing.
                // TODO: Figure out if the value of 01 represents something here.
                return _raw[2] >= 2;
            }
            set
            {
                if (value && IsNamed) return;
                else if (value && !IsNamed) _raw[2] = 2; // ID must be set separately. Default to 0
                else if (!value && IsNamed) _raw[2] = 0;
                else if (!value && !IsNamed) return;
            }
        }

        /// <summary>
        /// Doubles as forge level. 0x41 and up, I think
        /// </summary>
        public byte Uses
        {
            get
            {
                return (byte)((int)_raw[3] & ~0x40);
            }
            set
            {
                _raw[3] = (byte)(value | ((IsEquipped) ? (0x40) : (0x00)));
            }
        }
        
        public bool IsEquipped
        {
            get
            {
                return _raw[0x03].GetFlag(0x40);
            }
            set
            {
                _raw[0x03] = _raw[0x03].SetFlag(0x40, value);
            }
        }

        public string Hex()
        {
            return string.Format("{0:X2}{1:X2}{2:X2}{3:X2}",
                new object[] { _raw[0], _raw[1], _raw[2], _raw[3]});
        }

        public void Reparse(byte[] raw)
        {
            if (raw.Length != 4) throw new ArgumentException("Inventory items must be 4 bytes");
            _raw = raw;
        }
    }
}
