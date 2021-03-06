﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Q42.HueApi.Models.Groups
{
	[DataContract]
	public class Group
	{

		[DataMember]
		public string Id { get; set; }

		[DataMember(Name = "name")]
		public string Name { get; set; }

		/// <summary>
		/// Luminaire / Lightsource / LightGroup
		/// </summary>
		[JsonConverter(typeof(StringEnumConverter))]
		[DataMember(Name = "type")]
		public GroupType Type { get; set; }

		/// <summary>
		/// Category of the Room type. Default is "Other".
		/// </summary>
		[JsonConverter(typeof(StringEnumConverter))]
		[DataMember(Name = "class")]
		public RoomClass? Class { get; set; }

		/// <summary>
		/// As of 1.4. Uniquely identifies the hardware model of the luminaire. Only present for automatically created Luminaires.
		/// </summary>
		[DataMember(Name = "modelid")]
		public string ModelId { get; set; }

		/// <summary>
		/// The IDs of the lights that are in the group.
		/// </summary>
		[DataMember(Name = "lights")]
		public List<string> Lights { get; set; }

		/// <summary>
		/// The light state of one of the lamps in the group.
		/// </summary>
		[DataMember(Name = "action")]
		public State Action { get; set; }

		[DataMember(Name = "state")]
		public GroupState State { get; set; }

	}

	[DataContract]
	public class GroupState
	{
		[DataMember(Name = "any_on")]
		public bool? AnyOn { get; set; }

		[DataMember(Name = "all_on")]
		public bool? AllOn { get; set; }
	}

	/// <summary>
	/// Possible group types
	/// </summary>
	public enum GroupType
	{
		
		[EnumMember(Value = "LightGroup")]
		LightGroup,
		[EnumMember(Value = "Room")]
		Room,
		[EnumMember(Value = "Luminaire")]
		Luminaire,
		[EnumMember(Value = "LightSource")]
		LightSource
	}

	/// <summary>
	/// Possible room types
	/// </summary>
	public enum RoomClass
	{
		[EnumMember(Value = "Other")]
		Other,
		[EnumMember(Value = "Living room")]
		LivingRoom,
		[EnumMember(Value = "Kitchen")]
		Kitchen,
		[EnumMember(Value = "Dining")]
		Dining,
		[EnumMember(Value = "Bedroom")]
		Bedroom,
		[EnumMember(Value = "Kids bedroom")]
		KidsBedroom,
		[EnumMember(Value = "Bathroom")]
		Bathroom,
		[EnumMember(Value = "Nursery")]
		Nursery,
		[EnumMember(Value = "Recreation")]
		Recreation,
		[EnumMember(Value = "Office")]
		Office,
		[EnumMember(Value = "Gym")]
		Gym,
		[EnumMember(Value = "Hallway")]
		Hallway,
		[EnumMember(Value = "Toilet")]
		Toilet,
		[EnumMember(Value = "Front door")]
		FrontDoor,
		[EnumMember(Value = "Garage")]
		Garage,
		[EnumMember(Value = "Terrace")]
		Terrace,
		[EnumMember(Value = "Garden")]
		Garden,
		[EnumMember(Value = "Driveway")]
		Driveway,
		[EnumMember(Value = "Carport")]
		Carport
	}

}
