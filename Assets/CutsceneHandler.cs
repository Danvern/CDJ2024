using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using DG.Tweening;
using Unity.VisualScripting;

public class CutsceneHandler : MonoBehaviour
{
	[Header("Cutscene Settings")]
	[SerializeField] private List<Sprite> images;
	[SerializeField] private List<string> texts;
	[SerializeField] private Image imageHolder;
	[SerializeField] private TextMeshProUGUI textHolder;
	[SerializeField] private float textSpeed = 0.05f;
	[SerializeField] private float textWordWait = 0.25f;
	[SerializeField] private float textMinimumWait = 1f;
	[SerializeField] private float finalWait = 1f;
	[SerializeField] private float fadeDuration = 1f;
	[SerializeField] private UnityEvent onCutsceneEnd;

	[Header("Scrolling Mode Settings")]
	[SerializeField] private bool useScrollingMode = false;
	[SerializeField] private float scrollMaxOffset = 100f;
	[SerializeField] private float scrollTime = 10f;
	[SerializeField] private float scrollDelay = 2f;
	[SerializeField] private int scrollTrigger = 0;
	[SerializeField] private RectTransform imageHolderRectTransform;


	private int currentIndex = 0;
	private bool isTextScrolling = false;
	private bool isSkipping = false;
	private Coroutine textCoroutine;
	private WaitForSeconds autoScrollWait;

	private void Start()
	{
		textHolder.text = string.Empty;
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

			if (useScrollingMode)
			{
				imageHolder.sprite = currentImage;
				if (currentIndex == scrollTrigger)
					StartCoroutine(ScrollImage());
			}
			else if (currentImage != previousImage)
			{
				imageHolder.sprite = currentImage;
				yield return StartCoroutine(FadeImage(1));
			}

			// Start text scrolling
			textCoroutine = StartCoroutine(ScrollText(texts[currentIndex]));

			// Wait for text to finish
			yield return textCoroutine;

			if (!useScrollingMode && currentIndex + 1 < images.Count && images[currentIndex + 1] != currentImage)
			{
				yield return StartCoroutine(FadeImage(0));
			}

			previousImage = currentImage;
			currentIndex++;
		}

		yield return new WaitForSeconds(finalWait);
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
		isSkipping = false;
		float startWaitTime = Time.unscaledTime;
		float targetWaitTime = textWordWait * fullText.WordCount() + textMinimumWait;

		while (Time.unscaledTime - startWaitTime < targetWaitTime && !isSkipping)
		{
			yield return new WaitForSeconds(textSpeed);
		}

		yield return autoScrollWait;
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

	private IEnumerator ScrollImage()
	{
		// Delay before scrolling
		yield return new WaitForSeconds(scrollDelay);

		// Scroll the image downwards
		Vector2 initialPosition = imageHolderRectTransform.anchoredPosition;
		DOTween.To(() => initialPosition, x => imageHolderRectTransform.anchoredPosition = x, initialPosition.Add(y: scrollMaxOffset), scrollTime).SetEase(Ease.OutCubic).SetId(this);
		// while (true)
		// {
		// 	imageHolderRectTransform.anchoredPosition += Vector2.down * scrollTime * Time.deltaTime;
		// 	yield return null;
		// }
	}

	private void NextScene()
	{
		if (textCoroutine != null)
		{
			StopCoroutine(textCoroutine);
			if (currentIndex < texts.Count)
				textHolder.text = texts[currentIndex];
			else
				textHolder.text = "";
			isTextScrolling = false;
		}
	}

	void OnDisable()
	{
		DOTween.Kill(this);
	}
}
