using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineRendererSettings : MonoBehaviour
{
    [SerializeField] 
    LineRenderer rend;

    Vector3[] points;

    public LayerMask layerMask;

    public GameObject panel;
    public Image img;
    public Button btn;

    void Start()
    {

        img = panel.GetComponent<Image>();
        rend = gameObject.GetComponent<LineRenderer>();

        points = new Vector3[2];

        points[0] = Vector3.zero;

        points[1] = transform.position + new Vector3(0, 0, 20);

        rend.SetPositions(points);
        rend.enabled = true;
    }

    public bool AlignLineRenderer(LineRenderer rend)
    {
        bool hitBtn = false;
        Ray ray;
        ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, layerMask))
        {
            hitBtn = true;
            btn = hit.collider.gameObject.GetComponent<Button>();
            rend.startColor = Color.red;
            rend.endColor = Color.red;
            points[1] = transform.forward + new Vector3(0, 0, hit.distance);
        }
        else
        {
            hitBtn = false;
            rend.startColor = Color.green;
            rend.endColor = Color.green;
            points[1] = transform.forward + new Vector3(0, 0, 20);
        }

        rend.material.color = rend.startColor;
        rend.SetPositions(points);
        return hitBtn;
    }

    void Update()
    {
        if(AlignLineRenderer(rend) && Input.GetAxis("Submit") > 0)
        {
            btn.onClick.Invoke();
        }
    }

    public void ColorChangeOnClick()
    {
        if(btn != null)
        {
            if(btn.name == "START")
            {
                img.color = Color.red;
            }
        }
    }
}
