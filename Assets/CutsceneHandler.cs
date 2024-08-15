using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class CutsceneHandler : MonoBehaviour
{
	[Header("Cutscene Settings")]
	[SerializeField] private List<Sprite> images;
	[SerializeField] private List<string> texts;
	[SerializeField] private Image imageHolder;
	[SerializeField] private TextMeshProUGUI textHolder;
	[SerializeField] private float textSpeed = 0.05f;
	[SerializeField] private float fadeDuration = 1f;
	[SerializeField] private UnityEvent onCutsceneEnd;

	private int currentIndex = 0;
	private bool isTextScrolling = false;
	private bool isSkipping = false;
	private Coroutine textCoroutine;

	private void Start()
	{
		StartCutscene();
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
		{
			if (isTextScrolling)
			{
				isSkipping = true;
			}
			else
			{
				NextScene();
			}
		}
	}

	private void StartCutscene()
	{
		if (images.Count == 0 || texts.Count == 0 || imageHolder == null || textHolder == null)
		{
			Debug.LogError("CutsceneHandler: Ensure images, texts, imageHolder, and textHolder are all set and contain elements.");
			return;
		}

		currentIndex = 0;
		StartCoroutine(PlayScene());
	}

	private IEnumerator PlayScene()
	{
		Sprite previousImage = null;

		while (currentIndex < images.Count && currentIndex < texts.Count)
		{
			Sprite currentImage = images[currentIndex];

			// Set image if it's different from the previous one
			if (currentImage != previousImage)
			{
				imageHolder.sprite = currentImage;

				// Fade in the image
				yield return StartCoroutine(FadeImage(1));
			}

			// Start text scrolling
			textCoroutine = StartCoroutine(ScrollText(texts[currentIndex]));

			// Wait for text to finish
			yield return textCoroutine;

			// Fade out the image only if the next image is different
			if (currentIndex + 1 < images.Count && images[currentIndex + 1] != currentImage)
			{
				yield return StartCoroutine(FadeImage(0));
			}

			// Move to the next scene
			previousImage = currentImage;
			currentIndex++;
		}

		// Trigger the end of the cutscene
		onCutsceneEnd.Invoke();
	}


	private IEnumerator ScrollText(string fullText)
	{
		textHolder.text = "";
		isTextScrolling = true;
		isSkipping = false;

		for (int i = 0; i < fullText.Length; i++)
		{
			if (isSkipping)
			{
				textHolder.text = fullText;
				break;
			}

			textHolder.text += fullText[i];
			yield return new WaitForSeconds(textSpeed);
		}

		isTextScrolling = false;
	}

	private IEnumerator FadeImage(float targetAlpha)
	{
		Color color = imageHolder.color;
		float startAlpha = color.a;
		float t = 0f;

		while (t < fadeDuration)
		{
			t += Time.deltaTime;
			color.a = Mathf.Lerp(startAlpha, targetAlpha, t / fadeDuration);
			imageHolder.color = color;
			yield return null;
		}

		color.a = targetAlpha;
		imageHolder.color = color;
	}

	private void NextScene()
	{
		if (textCoroutine != null)
		{
			StopCoroutine(textCoroutine);
			textHolder.text = texts[currentIndex];
			isTextScrolling = false;
		}
	}
}
