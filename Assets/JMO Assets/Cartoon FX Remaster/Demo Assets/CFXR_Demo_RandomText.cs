using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CartoonFX
{
	public class CFXR_Demo_RandomText : MonoBehaviour
	{
		public ParticleSystem partSystem;
		public CFXR_ParticleText_Runtime runtimeParticleText;

		void OnEnable()
		{
			InvokeRepeating("SetRandomText", 0f, 1.5f);
		}

		void OnDisable()
		{
			CancelInvoke("SetRandomText");
			partSystem.Clear(true);
		}

		void SetRandomText()
		{
			// set text size according to the damage amount
			int damage = Random.Range(10, 1000);
			runtimeParticleText.size = Mathf.Lerp(0.8f, 1.3f, damage / 1000f);

			// update text
			string text = damage.ToString();
			runtimeParticleText.GenerateText(text);

			partSystem.Play(true);
		}
	}
}