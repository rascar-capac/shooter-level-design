// GercStudio
// © 2018-2019

using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Rendering;

namespace GercStudio.USK.Scripts
{

    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(Rigidbody))]
    public class PickUp : MonoBehaviour
    {
        public int health_add = 20;
        public int ammo_add = 20;

        public string ammoType;
        public string pickUpId;

        public Texture KitImage;

        public enum TypeOfPickUp
        {
            Health,
            Ammo,
            Weapon
        }

        public enum PickUpMethod
        {
            Collider,
            Raycast
        }

        public PickUpMethod Method;
        public TypeOfPickUp PickUpType;
        
        public AudioClip PickUpAudio;

        public Vector3 ColliderSize = Vector3.one;

        [Range(1, 100)] public int distance = 10;
        [Range(1, 8)] public int Slots = 1;

        public BoxCollider pickUpArea;

        private GameObject target;

        private void OnEnable()
        {
            var rigidbody = GetComponent<Rigidbody>();
            var collider = GetComponent<BoxCollider>();

            rigidbody.isKinematic = false;
            rigidbody.useGravity = true;
            collider.isTrigger = false;
            
            if (Method == PickUpMethod.Collider && !pickUpArea)
            {
                pickUpArea = gameObject.AddComponent<BoxCollider>();
                pickUpArea.size = ColliderSize;
                pickUpArea.isTrigger = true;
            }
        }

        public void PickUpObject(GameObject character)
        {
            if (!character.GetComponent<Controller>()) return;
            
            Destroy(pickUpArea);

            target = character;
            switch (PickUpType)
            {
                case TypeOfPickUp.Health:
                {
                    var healthKit = new InventoryManager.Kit
                    {
                        AddedValue = health_add,
                        Image = KitImage
                    };
                    
                    if (PickUpAudio & character.GetComponent<AudioSource>())
                    {
                        character.GetComponent<AudioSource>().PlayOneShot(PickUpAudio);
                    }
                    target.GetComponent<InventoryManager>().HealthKits.Add(healthKit);
                    Destroy(gameObject);
                    break;
                }
                case TypeOfPickUp.Ammo:
                {
                    var hasWeapon = false;
                    var weaponManager = target.GetComponent<InventoryManager>();
                    for (var i = 0; i < 8; i++)
                    {
                        foreach (var weapon in weaponManager.slots[i].weaponsInInventory)
                        {
                            var weaponController = weapon.weapon.GetComponent<WeaponController>();
                            if (ammoType == "")
                            {
                                weapon.WeaponAmmoKits.Add(new InventoryManager.Kit()
                                    {AddedValue = ammo_add, Image = KitImage, ammoType = ammoType, PickUpId = pickUpId});
                                hasWeapon = true;
                            }
                            else if (weaponController.Attacks[weaponController.currentAttack].AmmoType == ammoType)
                            {
                                if (weaponController.Attacks.All(attack => attack.AttackType != WeaponsHelper.TypeOfAttack.Grenade))
                                {
                                    weapon.WeaponAmmoKits.Add(new InventoryManager.Kit {AddedValue = ammo_add, Image = KitImage, ammoType = ammoType, PickUpId = pickUpId});
                                    hasWeapon = true;
                                }
                            }
                        }
                    }
                    
                    if (!hasWeapon)
                    {
                        foreach (var grenade in weaponManager.Grenades)
                        {
                            if (grenade.GrenadeScript.Attacks[grenade.GrenadeScript.currentAttack].AmmoType == ammoType)
                            {
                                grenade.grenadeAmmo += ammo_add;
                                hasWeapon = true;
                            }
                        }
                    }
                    
                    if (PickUpAudio & character.GetComponent<AudioSource>())
                    {
                        character.GetComponent<AudioSource>().PlayOneShot(PickUpAudio);
                    }

                    if (!hasWeapon)
                        weaponManager.ReserveAmmo.Add(new InventoryManager.Kit(){AddedValue = ammo_add, Image = KitImage, ammoType = ammoType, PickUpId = pickUpId});
                    
                    Destroy(gameObject);
                    break;
                }
                case TypeOfPickUp.Weapon:
                {
                    var weaponController = GetComponent<WeaponController>();
                    var weaponManager = target.GetComponent<InventoryManager>();
                    var controller = target.GetComponent<Controller>();
                    enabled = false;

                    switch (Slots)
                    {
                        case 1:
                            PlaceWeaponInInventory(0, weaponManager, controller);
                            break;
                        case 2:
                            PlaceWeaponInInventory(1, weaponManager, controller);
                            break;
                        case 3:
                            PlaceWeaponInInventory(2, weaponManager, controller);
                            break;
                        case 4:
                            PlaceWeaponInInventory(3, weaponManager, controller);
                            break;
                        case 5:
                            PlaceWeaponInInventory(4, weaponManager, controller);
                            break;
                        case 6:
                            PlaceWeaponInInventory(5, weaponManager, controller);
                            break;
                        case 7:
                            PlaceWeaponInInventory(6, weaponManager, controller);
                            break;
                        case 8:
                            PlaceWeaponInInventory(7, weaponManager, controller);
                            break;
                    }

                    if (weaponController.PickUpWeaponAudio & character.GetComponent<AudioSource>())
                    {
                        character.GetComponent<AudioSource>().PlayOneShot(weaponController.PickUpWeaponAudio);
                    }

                    transform.parent = controller.BodyObjects.RightHand;
                    weaponController.Controller = controller;
                    weaponController.WeaponManager = weaponManager;
                    weaponController.Parent = controller.transform;
                    
                    weaponController.enabled = true;
                    weaponController.PickUpWeapon = false;
                    
                    break;
                }
            }
        }

        void PlaceWeaponInInventory(int slotNumber, InventoryManager weaponManager, Controller controller)
        {
            var weapon = new InventoryManager.Weapon()
                {weapon = gameObject, WeaponAmmoKits = new List<InventoryManager.Kit>()};

            List<InventoryManager.Kit> removeWeapons = new List<InventoryManager.Kit>();

            foreach (var kit in weaponManager.ReserveAmmo)
            {
                var weaponController = weapon.weapon.GetComponent<WeaponController>();
                
                if (kit.ammoType == weaponController.Attacks[weaponController.currentAttack].AmmoType)
                {
                    weapon.WeaponAmmoKits.Add(kit);
                    removeWeapons.Add(kit);
                }
            }

            if (removeWeapons.Count > 0)
            {
                foreach (var removeWeapon in removeWeapons)
                {
                    if (weaponManager.ReserveAmmo.Contains(removeWeapon))
                        weaponManager.ReserveAmmo.Remove(removeWeapon);
                }

                removeWeapons.Clear();
            }

            weaponManager.slots[slotNumber].weaponsInInventory.Add(weapon);

            weaponManager.slots[slotNumber].currentWeaponInSlot =
                weaponManager.slots[slotNumber].weaponsInInventory.Count - 1;

            if (weaponManager.hasAnyWeapon)
            {
                if (weaponManager.currentSlot == slotNumber)
                    weaponManager.Switch(slotNumber);
                else
                {
                    gameObject.GetComponent<WeaponController>().Controller = controller;
                    gameObject.SetActive(false);
                }
            }
            else
            {
                weaponManager.currentSlot = slotNumber;
                weaponManager.Switch(slotNumber);
            }
        }
        
#if UNITY_EDITOR
        
        void OnDrawGizmosSelected()
        {
            Handles.zTest = CompareFunction.Greater;
            Handles.color = new Color32(255, 153, 0, 50);

            if (Method == PickUpMethod.Collider)
            {
                Handles.matrix = transform.localToWorldMatrix;
                Handles.DrawWireCube(Vector3.zero, ColliderSize);
            }
            else
            {
                Handles.DrawWireDisc(transform.position, Vector3.up, distance);
            }


            Handles.zTest = CompareFunction.Less;
            Handles.color = new Color32(255, 153, 0, 255);

            if (Method == PickUpMethod.Collider)
            {
                Handles.DrawWireCube(Vector3.zero, ColliderSize);
                Handles.matrix = transform.localToWorldMatrix;
            }
            else
            {
                Handles.DrawWireDisc(transform.position, Vector3.up, distance);
            }
        }
#endif
    }
}




