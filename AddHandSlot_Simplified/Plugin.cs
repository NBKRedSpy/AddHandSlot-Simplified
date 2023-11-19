using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace AddHandSlot_Simplified;

[BepInPlugin("NBK_RedSpy_Pikachu.AddHandSlot_Simplified", "Add Hand Slot_Simplified", "1.4.2")]
public class Plugin : BaseUnityPlugin
{
	private static Plugin Instance;

	//private static GameStat HandSlotNum;

	private static bool AllowInventoryDoubleLine = true;

	private ConfigEntry<bool> DoubleLine_EnableLocation;

	private ConfigEntry<bool> DoubleLine_EnableBase;

	private ConfigEntry<bool> DoubleLine_EnableHand;

	private ConfigEntry<bool> DoubleLine_EnableInventory;

	private ConfigEntry<bool> DoubleLine_EnableEquipment;

	private ConfigEntry<bool> SlotScale_EnableLocation;

	private ConfigEntry<bool> SlotScale_EnableBase;

	private ConfigEntry<bool> SlotScale_EnableHand;

	private ConfigEntry<bool> SlotScale_EnableBlueprint;

	private ConfigEntry<bool> SlotScale_EnableInventory;

	private ConfigEntry<bool> SlotScale_EnableEquipment;

	private ConfigEntry<bool> StatScale_EnableBar;

	private ConfigEntry<bool> Special_EnableInventoryDynamicDoubleLine;

	private ConfigEntry<bool> Special_EnableStatusBarElongate;

	private void Awake()
	{
		//if (!(AccessTools.TypeByName("ModLoader.ModPack") != null) || !IsDisable("AddHandSlot"))
		//{
		Instance = this;
		Harmony harmony = Harmony.CreateAndPatchAll(typeof(Plugin));
		base.Logger.LogInfo("Plugin [Add Hand Slot] is loaded!");
		DoubleLine_EnableLocation = base.Config.Bind("DoubleLine", "EnableLocation", defaultValue: true, "双行环境槽位（测试功能，谨慎使用，开启后将强制修改环境槽位的尺寸）");
		DoubleLine_EnableBase = base.Config.Bind("DoubleLine", "EnableBase", defaultValue: true, "双行基础槽位（测试功能，谨慎使用，开启后将强制修改基础槽位的尺寸）");
		DoubleLine_EnableHand = base.Config.Bind("DoubleLine", "EnableHand", defaultValue: false, "双行手牌槽位（测试功能，谨慎使用，开启后将强制修改手牌槽位的尺寸）");
		DoubleLine_EnableInventory = base.Config.Bind("DoubleLine", "EnableInventory", defaultValue: true, "双行容器槽位（测试功能，谨慎使用，开启后将强制修改容器槽位的尺寸）");
		DoubleLine_EnableEquipment = base.Config.Bind("DoubleLine", "EnableEquipment", defaultValue: true, "双行装备槽位（测试功能，谨慎使用，开启后将强制修改装备槽位的尺寸）");
		SlotScale_EnableLocation = base.Config.Bind("SlotScale", "EnableLocation", defaultValue: false, "修改环境槽位的尺寸");
		SlotScale_EnableBase = base.Config.Bind("SlotScale", "EnableBase", defaultValue: false, "修改基础槽位的尺寸");
		SlotScale_EnableHand = base.Config.Bind("SlotScale", "EnableHand", defaultValue: false, "修改手牌槽位的尺寸");
		SlotScale_EnableBlueprint = base.Config.Bind("SlotScale", "EnableBlueprint", defaultValue: false, "修改蓝图槽位的尺寸");
		SlotScale_EnableInventory = base.Config.Bind("SlotScale", "EnableInventory", defaultValue: false, "修改容器槽位的尺寸");
		SlotScale_EnableEquipment = base.Config.Bind("SlotScale", "EnableEquipment", defaultValue: false, "修改装备槽位的尺寸");
		StatScale_EnableBar = base.Config.Bind("StatScale", "EnableBar", defaultValue: false, "修改状态栏的尺寸");
		Special_EnableInventoryDynamicDoubleLine = base.Config.Bind("Special", "EnableInventoryDynamicDoubleLine", defaultValue: false, "容器动态双行槽位（需同时启用双行容器槽位生效，启用后仅当容器槽位数量大于8时才会按双行显示）");
		Special_EnableStatusBarElongate = base.Config.Bind("Special", "EnableStatusBarElongate", defaultValue: false, "状态条延长（仅当启用修改状态栏的尺寸时生效）");
		if (!DoubleLine_EnableHand.Value && !DoubleLine_EnableInventory.Value && !DoubleLine_EnableBase.Value && !DoubleLine_EnableLocation.Value)
		{
			harmony.Unpatch(AccessTools.Method(typeof(DynamicViewLayoutGroup), "GetElementPosition"), typeof(Plugin).GetMethod("DynamicViewLayoutGroup_GetElementPosition_Postfix"));
		}
		//}
	}

	//private static bool IsDisable(string mod_name)
	//{
	//	if (ModLoader.ModPacks.TryGetValue(mod_name, out var value) && value != null)
	//	{
	//		return !value.EnableEntry.Value;
	//	}
	//	return true;
	//}

	//[HarmonyPostfix]
	//[HarmonyPatch(typeof(GameLoad), "LoadMainGameData")]
	//public static void GameLoad_LoadMainGameData_Postfix()
	//{
	//	CharacterPerk fromID = UniqueIDScriptable.GetFromID<CharacterPerk>("97dbfde7a82f4bc69421363adffeb0b5");
	//	if ((bool)fromID)
	//	{
	//		fromID.StartingStatModifiers[0].ValueModifier = new Vector2(Instance.Config_AddSlotNum.Value, Instance.Config_AddSlotNum.Value);
	//	}
	//	HandSlotNum = UniqueIDScriptable.GetFromID<GameStat>("3ed9754d13824a918badb45308baff0c");
	//	if (Instance.Config_EnableModifyEncumbrance.Value)
	//	{
	//		ModifyEncumbrance();
	//	}
	//}

	//private static void ModifyEncumbrance()
	//{
	//	GameStat fromID = UniqueIDScriptable.GetFromID<GameStat>("21574a6120f4d3c4b913c69987e2ff06");
	//	if (!(fromID == null))
	//	{
	//		StatStatus[] statuses = fromID.Statuses;
	//		if (statuses != null && statuses.Length == 4)
	//		{
	//			int num = (int)fromID.MinMaxValue.y + Instance.Config_AddEncumbranceNum.Value;
	//			fromID.MinMaxValue.y = num;
	//			fromID.VisibleValueRange.y = num;
	//			fromID.Statuses[0].ValueRange = new Vector2Int((int)((double)num * 0.5) + 1, (int)((double)num * 0.75));
	//			fromID.Statuses[1].ValueRange = new Vector2Int((int)((double)num * 0.75) + 1, (int)((double)num * 0.875));
	//			fromID.Statuses[2].ValueRange = new Vector2Int((int)((double)num * 0.875) + 1, num - 1);
	//			fromID.Statuses[3].ValueRange = new Vector2Int(num, num);
	//		}
	//	}
	//}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(GraphicsManager), "Init")]
	public static void GraphicsManager_Init_Prefix(GraphicsManager __instance)
	{
		if (Instance.DoubleLine_EnableHand.Value)
		{
			Transform parent = __instance.ItemSlotsLine.transform.parent;
			parent.Find("HandGroup").localScale = new Vector3(0.5f, 0.5f, 1f);
			parent.Find("HandGroupLogic").localScale = new Vector3(0.5f, 0.5f, 1f);
		}
		else if (Instance.SlotScale_EnableHand.Value)
		{
			Transform parent2 = __instance.ItemSlotsLine.transform.parent;
			parent2.Find("HandGroup").localScale = new Vector3(0.7f, 0.7f, 1f);
			parent2.Find("HandGroupLogic").localScale = new Vector3(0.7f, 0.7f, 1f);
		}
		if (Instance.DoubleLine_EnableLocation.Value)
		{
			Transform parent3 = __instance.LocationSlotsLine.transform.parent;
			parent3.localScale = new Vector3(0.5f, 0.5f, 1f);
			parent3.GetComponent<RectTransform>().sizeDelta = new Vector2(1310f, 360f);
		}
		else if (Instance.SlotScale_EnableLocation.Value)
		{
			Transform parent4 = __instance.LocationSlotsLine.transform.parent;
			parent4.localScale = new Vector3(0.7f, 0.7f, 1f);
			parent4.GetComponent<RectTransform>().sizeDelta = new Vector2(550f, 0f);
		}
		if (Instance.DoubleLine_EnableBase.Value)
		{
			Transform parent5 = __instance.BaseSlotsLine.transform.parent;
			parent5.localScale = new Vector3(0.5f, 0.5f, 1f);
			parent5.GetComponent<RectTransform>().sizeDelta = new Vector2(1160f, 360f);
		}
		else if (Instance.SlotScale_EnableBase.Value)
		{
			Transform parent6 = __instance.BaseSlotsLine.transform.parent;
			parent6.localScale = new Vector3(0.7f, 0.7f, 1f);
			parent6.GetComponent<RectTransform>().sizeDelta = new Vector2(380f, 0f);
		}
		if (Instance.SlotScale_EnableBlueprint.Value)
		{
			Transform parent7 = __instance.BlueprintSlotsLine.transform.parent;
			parent7.localScale = new Vector3(0.7f, 0.7f, 1f);
			parent7.GetComponent<RectTransform>().sizeDelta = new Vector2(620f, 15f);
		}
		if (Instance.DoubleLine_EnableInventory.Value)
		{
			Transform obj = __instance.InventoryInspectionPopup.transform.Find("ShadowAndPopupWithTitle/Content/InspectionGroup/InventoryGroup/Inventory/InventoryViewport");
			obj.localScale = new Vector3(0.5f, 0.5f, 1f);
			obj.GetComponent<RectTransform>().sizeDelta = new Vector2(1175f, 360f);
		}
		else if (Instance.SlotScale_EnableInventory.Value)
		{
			Transform obj2 = __instance.InventoryInspectionPopup.transform.Find("ShadowAndPopupWithTitle/Content/InspectionGroup/InventoryGroup/Inventory/InventoryViewport");
			obj2.localScale = new Vector3(0.7f, 0.7f, 1f);
			obj2.GetComponent<RectTransform>().sizeDelta = new Vector2(500f, 0f);
		}
		if (Instance.DoubleLine_EnableEquipment.Value)
		{
			Transform parent8 = __instance.CharacterWindow.EquipmentSlotsLine.transform.parent;
			parent8.localScale = new Vector3(0.5f, 0.5f, 1f);
			parent8.GetComponent<RectTransform>().sizeDelta = new Vector2(1400f, 400f);
		}
		else if (Instance.SlotScale_EnableEquipment.Value)
		{
			Transform parent9 = __instance.CharacterWindow.EquipmentSlotsLine.transform.parent;
			parent9.localScale = new Vector3(0.7f, 0.7f, 1f);
			parent9.GetComponent<RectTransform>().sizeDelta = new Vector2(600f, -28.1044f);
		}
		Transform transform = MBSingleton<GameManager>.Instance.DraggingPlane.Find("DraggingParent");
		if (Instance.DoubleLine_EnableHand.Value || Instance.DoubleLine_EnableInventory.Value || Instance.DoubleLine_EnableBase.Value || Instance.DoubleLine_EnableLocation.Value)
		{
			transform.localScale = new Vector3(0.5f, 0.5f, 1f);
		}
		else if (Instance.SlotScale_EnableHand.Value || Instance.SlotScale_EnableLocation.Value || Instance.SlotScale_EnableBase.Value || Instance.SlotScale_EnableBlueprint.Value || Instance.SlotScale_EnableInventory.Value || Instance.SlotScale_EnableEquipment.Value)
		{
			transform.localScale = new Vector3(0.7f, 0.7f, 1f);
		}
		if (Instance.StatScale_EnableBar.Value)
		{
			Transform parent10 = MBSingleton<GraphicsManager>.Instance.ImportantStatusGraphicsParent.parent;
			parent10.localScale = new Vector3(0.7f, 0.7f, 1f);
			RectTransform component = parent10.GetComponent<RectTransform>();
			component.sizeDelta = new Vector2(417f, component.sizeDelta.y);
		}
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(DynamicViewLayoutGroup), "GetElementPosition")]
	public static void DynamicViewLayoutGroup_GetElementPosition_Postfix(DynamicViewLayoutGroup __instance, int _Index, ref Vector3 __result)
	{
		int num = 1;
		Transform transform;
		Vector2 vector;
		Vector2 vector2;
		if (__instance == MBSingleton<GraphicsManager>.Instance.ItemSlotsLine && Instance.DoubleLine_EnableHand.Value)
		{
			transform = MBSingleton<GraphicsManager>.Instance.ItemsParent;
			num = 2;
			vector = new Vector2(0f, -325f);
			vector2 = new Vector2(55f, 162.5f);
		}
		else if (__instance == MBSingleton<GraphicsManager>.Instance.BaseSlotsLine && Instance.DoubleLine_EnableBase.Value)
		{
			transform = MBSingleton<GraphicsManager>.Instance.BaseParent;
			num = 2;
			vector = new Vector2(0f, -325f);
			vector2 = new Vector2(50f, 360f);
		}
		else if (__instance == MBSingleton<GraphicsManager>.Instance.LocationSlotsLine && Instance.DoubleLine_EnableLocation.Value)
		{
			transform = MBSingleton<GraphicsManager>.Instance.LocationParent;
			vector = new Vector2(0f, -325f);
			vector2 = new Vector2(0f, 180f);
		}
		else if (__instance == MBSingleton<GraphicsManager>.Instance.InventoryInspectionPopup.InventorySlotsLine && Instance.DoubleLine_EnableInventory.Value && AllowInventoryDoubleLine)
		{
			transform = ((DynamicViewLayoutGroup)MBSingleton<GraphicsManager>.Instance.InventoryInspectionPopup.InventorySlotsLine).RectTr;
			vector = new Vector2(0f, -325f);
			vector2 = new Vector2(0f, 170f);
		}
		else
		{
			if (!(__instance == MBSingleton<GraphicsManager>.Instance.CharacterWindow.EquipmentSlotsLine) || !Instance.DoubleLine_EnableEquipment.Value)
			{
				return;
			}
			transform = MBSingleton<GraphicsManager>.Instance.CharacterWindow.EquipmentsParent;
			vector = new Vector2(0f, -325f);
			vector2 = new Vector2(0f, 170f);
		}
		if (transform == null)
		{
			return;
		}
		List<DynamicViewExtraSpace> extraSpaces = __instance.ExtraSpaces;
		if (extraSpaces == null || extraSpaces.Count <= 0)
		{
			return;
		}
		int num2 = __instance.AllElements.Count - __instance.InactiveElements;
		if (num2 % 2 == 1)
		{
			if (__instance == MBSingleton<GraphicsManager>.Instance.ItemSlotsLine && Instance.DoubleLine_EnableHand.Value)
			{
				vector2.x = 0f;
			}
			else
			{
				num2++;
			}
		}
		__instance.Size = __instance.Spacing * 0.5f * (float)num2;
		if (__instance.ExtraSpaces != null)
		{
			foreach (DynamicViewExtraSpace extraSpace in __instance.ExtraSpaces)
			{
				__instance.Size += extraSpace.Space;
			}
		}
		if ((bool)__instance.AddedSize && __instance.AddedSize.gameObject.activeSelf)
		{
			if (__instance.LayoutOrientation == RectTransform.Axis.Horizontal)
			{
				__instance.Size += __instance.AddedSize.rect.width;
			}
			else
			{
				__instance.Size += __instance.AddedSize.rect.height;
			}
		}
		if (__instance.LayoutOrientation == RectTransform.Axis.Horizontal)
		{
			__instance.Size += __instance.Padding.left + __instance.Padding.right;
		}
		else
		{
			__instance.Size += __instance.Padding.top + __instance.Padding.right;
		}
		__instance.Size = Mathf.Max(__instance.Size, __instance.MinSize);
		transform.GetComponent<RectTransform>().sizeDelta = new Vector2(__instance.Size, 0f);
		float num3 = 0f;
		foreach (DynamicViewExtraSpace item in extraSpaces)
		{
			if (item.AtIndex <= _Index)
			{
				num3 += item.Space;
			}
		}
		__result = __instance.LayoutOriginPos / num + __instance.LayoutDirection * (__instance.Spacing * (float)(_Index / 2) + num3) + vector * (_Index % 2) + vector2;
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(InspectionPopup), "Setup", new Type[] { typeof(InGameCardBase) })]
	public static void InspectionPopup_Setup_Postfix(InspectionPopup __instance)
	{
		if (__instance != MBSingleton<GraphicsManager>.Instance.InventoryInspectionPopup)
		{
			return;
		}
		Transform transform = __instance.transform.Find("ShadowAndPopupWithTitle/Content/InspectionGroup/InventoryGroup/Inventory/InventoryViewport");
		CardLine inventorySlotsLine = __instance.InventorySlotsLine;
		if (!Instance.DoubleLine_EnableInventory.Value || !Instance.Special_EnableInventoryDynamicDoubleLine.Value)
		{
			AllowInventoryDoubleLine = true;
			transform.localScale = new Vector3(0.5f, 0.5f, 1f);
			transform.GetComponent<RectTransform>().sizeDelta = new Vector2(1175f, 360f);
			((DynamicViewLayoutGroup)inventorySlotsLine).RecalculateSize = true;
			return;
		}
		if (inventorySlotsLine.Count - ((DynamicViewLayoutGroup)inventorySlotsLine).InactiveElements > 8)
		{
			AllowInventoryDoubleLine = true;
			transform.localScale = new Vector3(0.5f, 0.5f, 1f);
			transform.GetComponent<RectTransform>().sizeDelta = new Vector2(1175f, 360f);
		}
		else
		{
			AllowInventoryDoubleLine = false;
			transform.localScale = new Vector3(0.7f, 0.7f, 1f);
			transform.GetComponent<RectTransform>().sizeDelta = new Vector2(500f, 0f);
		}
		((DynamicViewLayoutGroup)inventorySlotsLine).RecalculateSize = true;
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(StatStatusGraphics), "Awake")]
	public static void StatStatusGraphics_Awake_Postfix(StatStatusGraphics __instance)
	{
		if (Instance.Special_EnableStatusBarElongate.Value)
		{
			RectTransform obj = (RectTransform)__instance.transform;
			obj.sizeDelta = new Vector2(400f, obj.sizeDelta.y);
		}
	}
}
